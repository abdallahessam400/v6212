using Long.Login.Network.Login.Packets;
using Long.Login.States;
using System.Collections.Concurrent;
using System.Numerics;

namespace Long.Login.Managers
{
    public class UserManager
    {
        private static readonly ILogger logger = Log.ForContext<UserManager>();
        private static ConcurrentDictionary<Guid, User> users = new ();
        private static ConcurrentDictionary<uint, Guid> onlineUsers = new();
        public static int UserCount => users.Count;
        public static int OnlineCount => onlineUsers.Count;
        public static int MaxUserCount { get; private set; }
        public static bool AddUser(User user)
        {
            if (users.TryAdd(user.Guid, user))
            {
                MaxUserCount = Math.Max(MaxUserCount, UserCount);
                return true;
            }
            return false;
        }

        public static User GetUser(uint accountID)
        {
            return users.Values.FirstOrDefault(x => x.AccountID == accountID);
        }

        public static User GetUser(Guid requestId)
        {
            return users.TryGetValue(requestId, out User user) ? user : null;
        }

        public static bool RemoveUser(Guid idUser)
        {
            return users.TryRemove(idUser, out _);
        }

        public static bool RemoveUser(uint accountID)
        {
            User user = GetUser(accountID);
            if (user != null)
            {
                return RemoveUser(user.Guid);
            }
            return false;
        }

        /// <summary>
        ///     MsgFee Begin of the game server.
        /// </summary>
        public static void SetOnline(uint accountID, Guid realmID)
        {
            onlineUsers[accountID] = realmID;
        }

        /// <summary>
        ///     MsgFee End and Offline of the game server.
        /// </summary>
        public static void SetOffline(uint accountID, Guid realmID)
        {
            onlineUsers.TryRemove(new KeyValuePair<uint, Guid>(accountID, realmID));
        }

        public static Guid? GetOnlineRealm(uint accountID)
        {
            return onlineUsers.TryGetValue(accountID, out Guid realmID) ? realmID : null;
        }

        public static void RemoveRealm(Guid realmID)
        {
            foreach (var user in onlineUsers.Where(x => x.Value == realmID))
            {
                onlineUsers.TryRemove(user);
            }
        }

        public static async Task OnTimerAsync()
        {
            foreach (var user in users.Values)
            {
                if (user.HasExpired)
                {
                    logger.Debug("User {0} login request expired.", user.UserName);
                    if (user.Client.Socket.Connected)
                    {
                        await user.Client.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ValidationTimeout), () =>
                        {
                            user.Client.Disconnect();
                            return Task.CompletedTask;
                        });
                    }
                    RemoveUser(user.AccountID);
                }
            }
        }
    }
}
