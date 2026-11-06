using Long.Login.Database;
using Long.Login.Database.Entities;
using Long.Login.Database.Repositories;
using Long.Login.Managers;
using Long.Login.Network.Game.Packets;
using Long.Login.States;
using Long.Network.Packets;

namespace Long.Login.Network.Login.Packets
{
    public sealed class MsgAccount : MsgBase<LoginClient>
    {
        private static readonly ILogger logger = Log.ForContext<MsgAccount>();

        // Conquer.exe 0x851cc7
        private const int PACKET_SIZE = 312;

        // Packet Properties
        public string Username { get; private set; }
        public string Realm { get; private set; }

        /// <summary>
        ///     Decodes a byte packet into the packet structure defined by this message class.
        ///     Should be invoked to structure data from the client for processing. Decoding
        ///     follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            using var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            reader.BaseStream.Seek(8, SeekOrigin.Begin);
            // Conquer.exe 0x851c8f: there is no password in this packet, see MsgSrpChallenge
            Username = reader.ReadString(128);
            reader.BaseStream.Seek(136, SeekOrigin.Begin);
            Realm = reader.ReadString(16);
        }

        public override async Task ProcessAsync(LoginClient client)
        {
            LoginStatisticManager.IncreaseLogin();

            try
            {
                // one try for each connection
                if (client.SrpRequested || Length != PACKET_SIZE)
                {
                    logger.Warning("Invalid login request from {0}.", client.IpAddress);
                    client.Disconnect();
                    return;
                }

                client.SrpRequested = true;
                if (LoginAttemptGuard.IsBlocked(client.IpAddress, Username))
                {
                    logger.Warning("Login of {0} from {1} blocked, too many attempts.", Username, client.IpAddress);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.AccountMaxLoginAttempts);
                    return;
                }

                GameAccount gameAccount = AccountRepository.GetByUsername(Username);
                if (gameAccount == null)
                {
                    // the same answer of a wrong password, so the names can't be tested
                    logger.Warning("Username {0} do not exist.", Username);
                    LoginAttemptGuard.Fail(client.IpAddress, Username);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.InvalidPassword);
                    return;
                }

                Srp6 srp;
                try
                {
                    srp = new Srp6(gameAccount.UserName, gameAccount.SrpVerifier, gameAccount.Salt);
                }
                catch (ArgumentException)
                {
                    logger.Warning("User {0} has no SRP password, use /setpass.", Username);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.InvalidPassword);
                    return;
                }

                client.Srp = srp;
                client.PendingAccount = gameAccount;
                client.PendingRealm = Realm;
                srp.GetOutgoingFields(out byte[] serverPublic, out byte[] salt);
                await client.SendAsync(new MsgSrpChallenge(serverPublic, salt));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error on MsgAccount processing! {0}", ex.Message);
                if (client.Socket.Connected)
                {
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.PleaseTryAgainLater);
                }
            }
        }

        /// <summary>
        ///     The rest of the login, after the proof of the password.
        /// </summary>
        public static async Task CompleteLoginAsync(LoginClient client, GameAccount gameAccount, string realmName)
        {
            string Username = gameAccount.UserName;
            string Realm = realmName;

            try
            {
#if !DEBUG
                CityLocation userLocation = LocationRepository.GetLocation(client.IpAddress);
#endif

                if (gameAccount.Flag != 0 || gameAccount.Deleted != null)
                {
                    logger.Warning("User {0} is locked.", Username);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.AccountBanned);
                    return;
                }

#if DEBUG
                //if (gameAccount.AuthorityId == 1)
                //{
                //    logger.Warning("User {0} non cooperator account.", Username);
                //    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.NonCooperatorAccount);
                //    return;
                //}
#endif

                var realm = RealmManager.GetRealm(Realm);
                if (realm == null)
                {
                    logger.Warning("User {0} attempted to connecto to unexistent {1} realm.", Username, Realm);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.ServerDown);
                    return;
                }

                if (realm.IsClosing)
                {
                    logger.Warning("User {0} attempted to connect to closing realm {1}.", Username, Realm);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.ServerDown);
                    return;
                }

                // the game server lets the staff in when it is full, RoleManager.LoginUserAsync
                if (realm.IsFull && gameAccount.AuthorityId <= 1)
                {
                    logger.Warning("User {0} attempted to connect to full realm {1}.", Username, Realm);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.ServerFull);
                    return;
                }

                // Online on another realm: MsgFee Kickout, and the player tries again.
                // On the same realm the game server takes care of it, MsgLoginUserExchange.
                Guid? onlineRealmId = UserManager.GetOnlineRealm((uint)gameAccount.Id);
                if (onlineRealmId != null && onlineRealmId != realm.Id)
                {
                    Realm onlineRealm = RealmManager.GetRealm(onlineRealmId.Value);
                    if (onlineRealm != null)
                    {
                        logger.Warning("User {0} is online on realm {1}, kicking.", Username, onlineRealm.Name);
                        await onlineRealm.KickAccountAsync((uint)gameAccount.Id, 0);
                        await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.PleaseTryAgainLater);
                        return;
                    }
                }

                //if (!realm.IsProduction && gameAccount.AuthorityId == 1)
                //{
                //    logger.Warning("User {0} non cooperator account on not production realm.", Username);
                //    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.NonCooperatorAccount);
                //    return;
                //}

                byte vipLevel = 0;
                var vipData = VipRepository.GetAccountVip(gameAccount.Id);
                if (vipData != null)
                {
                    vipLevel = vipData.VipLevel;
                }

                client.AccountID = (uint)gameAccount.Id;
                client.Username = gameAccount.UserName;

#if !DEBUG
                await LoginRecordRepository.LoginRcdAsync(client, userLocation, true);
#endif

                User user = UserManager.GetUser(client.AccountID);
                if (user != null)
                {
                    // duplicated login request
                    logger.Warning("User {0} is already awaiting for a login response.", Username);
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.PleaseTryAgainLater);
                    return;
                }

                user = new User(client, gameAccount, realm);
                UserManager.AddUser(user);

                await realm.Client.SendAsync(new MsgLoginUserExchange
                {
                    Data = new MsgLoginUserExchange.LoginExchangeData
                    {
                        AccountId = client.AccountID,
                        AuthorityId = (ushort)gameAccount.AuthorityId,
                        IpAddress = client.IpAddress,
                        Request = client.Guid.ToString(),
                        VipLevel = vipLevel
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error on MsgAccount processing! {0}", ex.Message);
                if (client.Socket.Connected)
                {
                    await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.PleaseTryAgainLater);
                }
            }
        }
    }
}
