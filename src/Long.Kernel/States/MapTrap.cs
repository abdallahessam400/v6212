using Long.Database.Entities;
using Long.Kernel.Database.Repositories;
using Long.Kernel.Managers;
using Long.Kernel.Network.Game.Packets;
using Long.Kernel.Processors;
using Long.Kernel.Scripting.Action;
using Long.Kernel.States.Magics;
using Long.Kernel.States.Npcs;
using Long.Kernel.States.User;
using Long.Network.Packets;
using Long.Shared.Managers;
using Long.Shared.Mathematics;
using System.Drawing;
using static Long.Kernel.Network.Game.Packets.MsgInteract;
using static Long.Kernel.Network.Game.Packets.MsgMapItem;

namespace Long.Kernel.States
{
    public sealed class MapTrap : Role
    {
        private static readonly ILogger logger = Log.ForContext<MapTrap>();

        private readonly TimeOutMS fightTimer;
        private readonly TimeOut lifePeriodTimer;

        private DbTrap trap;
        private Role owner;
        private Magic magic;
        private int activeTimes;
        private Point castPosition;
        private List<Point> ddaLine;

        public MapTrap(DbTrap trap)
        {
            this.trap = trap;

            Identity = trap.Id;
            MapIdentity = trap.MapId;
            X = trap.PosX;
            Y = trap.PosY;

            lifePeriodTimer = new TimeOut();
            lifePeriodTimer.Clear();
            fightTimer = new TimeOutMS();
            fightTimer.Clear();
        }

        public uint Type => trap.TypeId;
        public uint IdAction => trap.Type.ActionId;
        public int ActiveTimes => activeTimes;
        public int RemainingActiveTimes { get; set; }
        public override byte Level
        {
            get => trap.Type.Level;
            set => trap.Type.Level = value;
        }
        public override int MinAttack => trap.Type.AttackMin;
        public override int MaxAttack => trap.Type.AttackMax;
        public override int MagicAttack => trap.Type.AttackMax;
        public TargetType AttackMode => (TargetType)trap.Type.AtkMode;
        public override int AttackSpeed => trap.Type.AttackSpeed;
        public ushort MagicType => trap.Type.MagicType;
        public int MagicHitRate => trap.Type.MagicHitrate;
        public int Size => trap.Type.Size;

        public TrapTypeSort Sort => (TrapTypeSort)trap.Type.Sort;

        public bool IsTrapSort => trap.Type.Sort <= 10;

        public bool IsAutoSort => !IsTrapSort;

        public override bool IsAlive => RemainingActiveTimes > 0;

        public bool IsInRange(Role target)
        {
            return GetDistance(target) <= Size;
        }

        #region Initialization
        public async Task<bool> InitializeAsync(Role owner = null, Magic magic = null)
        {
            trap.Type ??= await TrapTypeRepository.GetAsync(trap.TypeId);

            if (trap.Type == null)
            {
                logger.Information($"Trap has no type {trap.Type} [TrapId: {trap.Id}]");
                return false;
            }

            this.owner = owner;
            this.magic = magic;

            if (magic != null && trap.Type.ActiveTimes == 0)
            {
                RemainingActiveTimes = activeTimes = (int)magic.ActiveTimes;
            }
            else
            {
                RemainingActiveTimes = activeTimes = trap.Type.ActiveTimes;
            }

            if (owner != null)
            {
                castPosition = new Point(owner.X, owner.Y);
            }
            else
            {
                castPosition = new Point(trap.PosX, trap.PosY);
            }

            fightTimer.Startup(trap.Type.AttackSpeed);

            Mesh = trap.Look;

            idMap = trap.MapId;
            currentX = trap.PosX;
            currentY = trap.PosY;

            await EnterMapAsync();
            return true;
        }

        #endregion

        #region Map

        public override Task EnterMapAsync()
        {
            Map = MapManager.GetMap(MapIdentity);
            if (Map != null)
            {
                return Map.AddAsync(this);
            }
            return Task.CompletedTask;
        }

        public override async Task LeaveMapAsync()
        {
            await BroadcastRoomMsgAsync(new MsgMapItem
            {
                Identity = Identity,
                Itemtype = Mesh,
                MapX = X,
                MapY = Y,
                Mode = DropType.DropTrap
            }, false);

            if (Map != null)
            {
                WorldProcessor.Instance.Queue(Map.Partition, async () =>
                {
                    await Map.RemoveAsync(Identity);
                });
            }

            if (Identity >= IdentityManager.MAGICTRAPID_FIRST && Identity <= IdentityManager.MAGICTRAPID_LAST)
                IdentityManager.Traps.ReturnIdentity(Identity);
        }

        #endregion

        #region Trap Attack
        private async Task MagicAttackAsync()
        {
            if (magic == null || !IsAutoSort)
            {
                return;
            }

            switch (Sort)
            {
                case TrapTypeSort.BombTrap:
                    {
                        await ProcessTrapBombAsync(magic);
                        break;
                    }

                case TrapTypeSort.ThickLineTrap:
                    {
                        await ProcessThickLineTrapAsync(magic);
                        break;
                    }
            }
        }

        private async Task ProcessTrapBombAsync(Magic magic)
        {
            List<Role> roles = CollectTargetBomb(trap.Type.Size / 2);

            long battleExp = 0;
            var hitByMagic = MagicData.HitByMagic(magic);
            var user = owner as Character;
            var currentEvent = user?.GetCurrentEvent();
            foreach (Role target in roles)
            {
                if (magic.Ground != 0 && target.IsWing)
                {
                    continue;
                }

                var atkResult = await BattleSystem.CalcPowerAsync(hitByMagic, owner ?? this, target, magic.Power);
                int damage = atkResult.Damage;
                if (user?.IsLucky == true && await ChanceCalcAsync(1, 100))
                {
                    await user.SendEffectAsync("LuckyGuy", true);
                    damage *= 2;
                }

                damage += atkResult.ElementalDamage;

                await BroadcastRoomMsgAsync(new MsgInteract
                {
                    SenderIdentity = Identity,
                    TargetIdentity = target.Identity,
                    MagicType = magic.Type,
                    MagicLevel = magic.Level,
                    Data = damage,
                    PosX = X,
                    PosY = Y,
                    Action = MsgInteractType.Attack
                }, false);

                var lifeLost = (int)Math.Min(damage, target.Life);
                await target.BeAttackAsync(hitByMagic, this, damage, true);

                if (user != null && (target is Monster monster && monster.SpeciesType == 0 && !monster.IsGuard() && !monster.IsPkKiller() && !monster.IsRighteous() || (target as DynamicNpc)?.IsGoal() == true))
                {
                    battleExp += user.AdjustExperience(target, lifeLost, false);
                    if (!target.IsAlive)
                    {
                        var nBonusExp = (int)(target.MaxLife * 20 / 100d);
                        if (user.Team != null)
                        {
                            await user.Team.AwardMemberExpAsync(user.Identity, target, nBonusExp);
                        }
                        battleExp += user.AdjustExperience(target, nBonusExp, false);
                    }
                }

                if (user != null && target is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }

                if (currentEvent != null)
                {
                    await currentEvent.OnHitAsync(user, target, magic);
                }

                if (user != null)
                {
                    await user.CheckCrimeAsync(target);
                }

                if (!target.IsAlive)
                {
                    await KillAsync(target, MagicData.GetDieMode());
                }
            }

            if (currentEvent != null)
            {
                await currentEvent.OnAttackAsync(user);
            }

            if (user != null)
            {
                await user.MagicData.AwardExpAsync(0, battleExp, battleExp, magic);
            }
        }
        public List<Point> ThickLineCoordinates(Magic magic) => Calculations.DDA(castPosition.X, castPosition.Y, X, Y, (int)magic.Distance);

        private async Task ProcessThickLineTrapAsync(Magic magic)
        {
            
        }

        private List<Role> CollectTargetBomb(int range)
        {
            List<Role> targets = new();
            List<Role> setRoles = Map.Query9BlocksByPos(X, Y);
            foreach (Role target in setRoles)
            {
                if (target.Identity == Identity || target.Identity == owner?.Identity)
                {
                    continue;
                }

                if (target.GetDistance(X, Y) > range)
                {
                    continue;
                }

                bool isCharacter = target is Character;
                bool isMonster = target is Monster;
                if (!AttackMode.HasFlag(TargetType.User) && isCharacter)
                {
                    continue;
                }

                //if (!AttackMode.HasFlag(TargetType.Monster) && isMonster)
                //{
                //    continue;
                //}

                if (!isCharacter && !isMonster)
                {
                    continue;
                }

                Role role = owner ?? this;
                if (role.IsImmunity(target) || !target.IsAttackable(owner ?? this))
                {
                    continue;
                }

                targets.Add(target);
            }
            return targets;
        }

        public override async Task KillAsync(Role target, uint dieWay)
        {
            if (target == null)
                return;

            if (owner != null)
            {
                await owner.KillAsync(target, dieWay);
                return;
            }

            await BroadcastRoomMsgAsync(new MsgInteract
            {
                Action = MsgInteractType.Kill,
                SenderIdentity = Identity,
                TargetIdentity = target.Identity,
                PosX = target.X,
                PosY = target.Y,
                Data = (int)dieWay
            }, true);

            await target.BeKillAsync(this);
        }

        #endregion

        #region Timer

        public override async Task OnTimerAsync()
        {
            if (!IsAlive && !IsAutoSort)
                return;

            if (lifePeriodTimer.IsActive())
            {
                if (lifePeriodTimer.ToNextTime())
                {
                    await LeaveMapAsync();
                    lifePeriodTimer.Clear();
                }
            }

            if (IsAutoSort)
            {
                if (fightTimer.ToNextTick(AttackSpeed))
                {
                    if (ActiveTimes > 0)
                        RemainingActiveTimes--;

                    await MagicAttackAsync();

                    if ((ActiveTimes > 0 || IsAutoSort) && RemainingActiveTimes <= 0)
                    {
                        await LeaveMapAsync();
                        IdentityManager.Traps.ReturnIdentity(Identity);
                    }
                }
            }
        }

        #endregion

        #region Socket

        public override Task SendSpawnToAsync(Character player)
        {
            return player.SendAsync(new MsgMapItem
            {
                Identity = Identity,
                Itemtype = Mesh,
                MapX = X,
                MapY = Y,
                Mode = DropType.SynchroTrap,
                Color = (ushort)Sort
            });
        }

        #endregion

        [Flags]
        public enum TargetType
        {
            None,
            User = 1,
            Monster = 2,
            Passive = 4
        }

        public enum TrapTypeSort
        {
            SystemTrap = 1,
            TrapSort = 10,
            BombTrap = 11,
            ThickLineTrap = 12
        }
    }
}