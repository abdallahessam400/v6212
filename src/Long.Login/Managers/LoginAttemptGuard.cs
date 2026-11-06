using System.Collections.Concurrent;

namespace Long.Login.Managers
{
    /// <summary>
    ///     Stops the password guessing.
    /// </summary>
    public static class LoginAttemptGuard
    {
        private const int MAX_FAILS_PER_IP = 20;
        private const int MAX_FAILS_PER_ACCOUNT = 20;
        private const int WINDOW_SECONDS = 60;
        private const int BLOCK_SECONDS = 300;

        private sealed class Entry
        {
            public int Fails;
            public DateTime WindowStart = DateTime.UtcNow;
            public DateTime BlockedUntil;
        }

        private static readonly ConcurrentDictionary<string, Entry> entries = new();

        public static bool IsBlocked(string ipAddress, string username)
        {
            return IsBlocked(IpKey(ipAddress)) || IsBlocked(AccountKey(username));
        }

        public static void Fail(string ipAddress, string username)
        {
            Fail(IpKey(ipAddress), MAX_FAILS_PER_IP);
            Fail(AccountKey(username), MAX_FAILS_PER_ACCOUNT);
        }

        public static void Success(string ipAddress, string username)
        {
            entries.TryRemove(IpKey(ipAddress), out _);
            entries.TryRemove(AccountKey(username), out _);
        }

        public static void Cleanup()
        {
            DateTime now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                lock (entry.Value)
                {
                    if (entry.Value.BlockedUntil <= now
                        && (now - entry.Value.WindowStart).TotalSeconds >= WINDOW_SECONDS)
                    {
                        entries.TryRemove(entry.Key, out _);
                    }
                }
            }
        }

        private static bool IsBlocked(string key)
        {
            if (!entries.TryGetValue(key, out Entry entry))
            {
                return false;
            }

            lock (entry)
            {
                return entry.BlockedUntil > DateTime.UtcNow;
            }
        }

        private static void Fail(string key, int limit)
        {
            Entry entry = entries.GetOrAdd(key, _ => new Entry());
            lock (entry)
            {
                DateTime now = DateTime.UtcNow;
                if ((now - entry.WindowStart).TotalSeconds >= WINDOW_SECONDS)
                {
                    entry.WindowStart = now;
                    entry.Fails = 0;
                }

                if (++entry.Fails >= limit)
                {
                    entry.BlockedUntil = now.AddSeconds(BLOCK_SECONDS);
                }
            }
        }

        private static string IpKey(string ipAddress) => "ip:" + ipAddress;

        private static string AccountKey(string username) => "account:" + username.ToLowerInvariant();
    }
}
