using Long.Login.Database.Entities;
using Long.Login.Network.Game;
using Long.Login.Network.Game.Packets;
using Long.Network.Packets.Login;

namespace Long.Login.States
{
    public sealed class Realm
    {
        private const int FULL_MS = 35000;

        private long fullUntil;

        public Realm(
            RealmData realm,
            GameClient client
            )
        {
            Id = realm.RealmID;
            RealmId = realm.RealmIdx;
            Name = realm.Name;
            IpAddress = realm.GameIPAddress;
            Port = (int)realm.GamePort;
            IsProduction = realm.ProductionRealm;
            Client = client;
        }

        public Guid Id { get; }
        public int RealmId { get; }
        public string Name { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public bool IsProduction { get; }
        public GameClient Client { get; }

        // the players of QueryFee, every 30 seconds
        public int OnlineCount { get; set; }
        public bool IsClosing { get; set; }

        // MsgServer.exe repeats ServerFull every 30 seconds while it is full, it sends nothing when it has room
        public bool IsFull => Environment.TickCount64 < Volatile.Read(ref fullUntil);

        public void SetFull()
        {
            Volatile.Write(ref fullUntil, Environment.TickCount64 + FULL_MS);
        }

        public Task KickAccountAsync(uint accountId, uint reason)
        {
            return Client.SendAsync(new MsgLoginAction
            {
                Data = new MsgLoginAction<GameClient>.LoginActionPB
                {
                    ServerID = (uint)RealmId,
                    Action = (uint)MsgLoginAction.LoginActionEnum.UserFee,
                    Param = (uint)MsgLoginAction.LoginFeeType.Kickout,
                    Data = accountId,
                    Command = reason
                }
            });
        }
    }
}
