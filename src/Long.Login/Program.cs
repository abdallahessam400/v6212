using Long.Database;
using Long.Login.Database;
using Long.Login.Database.Entities;
using Long.Login.Database.Repositories;
using Long.Login.Managers;
using Long.Login.Network.Game.Packets;
using Long.Login.Network.Login;
using Long.Login.States;
using Long.Network.Services;
using Long.Shared.Helpers;
using Serilog;
using System.Reflection;

namespace Long.Login
{
    internal class Program
    {
        public static string Version => Assembly.GetEntryAssembly()?
                                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                                .InformationalVersion ?? "0.0.0.0";

        private static readonly ILogger logger = Log.ForContext<Program>();

        public static async Task<int> Main(params string[] args)
        {
            Console.Title = "Loading...";

            Console.WriteLine("\tLong: Account Server");
            Console.WriteLine($"\t\tCopyright 2023 Felipe Vieira Vendramini / 2024 -{DateTime.Now:yyyy} Abdallah Essam");
            Console.WriteLine($"\t\tVersion: {Version}");
            Console.WriteLine("\tSome Rights Reserved");
            Console.WriteLine();

            Logger.Initialize("AccServer");

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception, the server is going down.");
                Log.CloseAndFlush();
            };
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                logger.Error(e.Exception, "Unobserved task exception.");
                e.SetObserved();
            };

            logger.Information("Logger initialized with success!");
            logger.Information("Initializating account settings");

            ServerSettings settings = new ServerSettings(args);
            AbstractDbContext.Configuration = settings.Database;

            Console.WriteLine();
            logger.Information("Database settings: {0} {1} **** {2}", settings.Database.Hostname, settings.Database.Username, settings.Database.Schema);
            logger.Information("Login Listener settings:");
            logger.Information("\tPort: {0}", settings.Login.Port);
            logger.Information("\tRecv processors: {0}", settings.Login.Listener.RecvProcessors);
            logger.Information("\tSend processors: {0}", settings.Login.Listener.SendProcessors);
            logger.Information("Game Listener settings:");
            logger.Information("\tPort: {0}", settings.Game.Port);
            logger.Information("\tRecv processors: {0}", settings.Game.Listener.RecvProcessors);
            logger.Information("\tSend processors: {0}", settings.Game.Listener.SendProcessors);
            Console.WriteLine();

            var tasks = new List<Task>
            {
                RandomnessService.Instance.StartAsync(CancellationToken.None)
            };
            Task.WaitAll(tasks.ToArray());

            if (!await Kernel.InitializeAsync(settings))
            {
                logger.Fatal("Could not initialize server!!!!");
                return 1;
            }

            await CommandLineAsync();
            return 0;
        }

        private static async Task CommandLineAsync()
        {
            string command;
            do
            {
                command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                {
                    continue;
                }
                try
                {
                    await ProcessCommandAsync(command);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error [{1}] running command {0}", command, ex.Message);
                }
            }
            while (!"exit".Equals(command));
        }

        private static async Task ProcessCommandAsync(string command)
        {
            string[] splitCmd = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (splitCmd.Length == 0)
            {
                return;
            }

            switch (splitCmd[0])
            {
                case "/newuser":
                    {
                        string[] args = splitCmd.Length < 2 ? [] : splitCmd[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (args.Length != 2)
                        {
                            Console.WriteLine(@"newuser username password");
                            break;
                        }

                        string username = args[0];
                        string salt = Srp6.CreateSaltHex();

                        if (AccountRepository.GetByUsername(username) != null)
                        {
                            Console.WriteLine(@"The required username is already in use.");
                            break;
                        }

                        GameAccount account = new GameAccount
                        {
                            UserName = username,
                            Password = GameAccount.HashPassword(args[1], salt),
                            Salt = salt,
                            SrpVerifier = Srp6.CreateVerifierHex(args[1], salt),
                            AuthorityId = 1,
                            MacAddress = string.Empty,
                            IpAddress = "127.0.0.1",
                            Created = DateTime.Now
                        };

                        using var db = new ServerDbContext();
                        db.GameAccounts.Add(account);
                        db.SaveChanges();
                        Console.WriteLine(@"new account Register.");
                        break;
                    }

                case "/setpass":
                    {
                        string[] args = splitCmd.Length < 2 ? [] : splitCmd[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (args.Length != 2)
                        {
                            Console.WriteLine(@"setpass username password");
                            break;
                        }

                        using var db = new ServerDbContext();
                        GameAccount account = db.GameAccounts.FirstOrDefault(x => x.UserName == args[0]);
                        if (account == null)
                        {
                            Console.WriteLine(@"The account does not exist.");
                            break;
                        }

                        account.Salt = Srp6.CreateSaltHex();
                        account.Password = GameAccount.HashPassword(args[1], account.Salt);
                        account.SrpVerifier = Srp6.CreateVerifierHex(args[1], account.Salt);
                        db.SaveChanges();
                        Console.WriteLine(@"password changed.");

                        // MsgServer.exe 0x14005ece4: the player leaves with the message of the client
                        Guid? realmId = UserManager.GetOnlineRealm((uint)account.Id);
                        Realm realm = realmId == null ? null : RealmManager.GetRealm(realmId.Value);
                        if (realm != null)
                        {
                            await realm.KickAccountAsync((uint)account.Id, MsgLoginAction.KICK_CHANGE_PASSWORD);
                        }

                        break;
                    }

                case "/version":
                    {
                        Console.WriteLine($"Version: {Version}");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown command.");
                        break;
                    }
            }
        }
    }
}