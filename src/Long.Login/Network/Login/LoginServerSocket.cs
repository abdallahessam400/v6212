using Long.Login.Managers;
using Long.Login.Network.Login.Packets;
using Long.Network.Packets;
using Long.Network.Services;
using Long.Network.Sockets;
using System.Net.Sockets;

namespace Long.Login.Network.Login
{
    public sealed class LoginServerSocket : TcpServerListener<LoginClient>
    {
        private static readonly ILogger logger = Log.ForContext<LoginServerSocket>();

        public static LoginServerSocket Instance { get; private set; }

        public LoginServerSocket(ServerSettings serverSettings)
            : base(50,
                  maxConnPerIp: 20,
                  maxAttemptsPerMinute: 60,
                  readProcessors: serverSettings.Login.Listener.RecvProcessors,
                  writeProcessors: serverSettings.Login.Listener.SendProcessors)
        {            
            Instance = this;
        }

        protected override async Task<LoginClient> AcceptedAsync(Socket socket, Memory<byte> buffer)
        {
            uint readProcessor = packetProcessor.SelectReadPartition();
            uint writeProcessor = packetProcessor.SelectWritePartition();
            LoginClient loginClient = new LoginClient(socket, buffer, readProcessor, writeProcessor);
            loginClient.Seed = (uint)(await RandomnessService.Instance.NextIntegerAsync(10000, int.MaxValue));
            await loginClient.SendAsync(new MsgEncryptCode(loginClient.Seed));
            return loginClient;
        }

        protected override async Task ProcessAsync(LoginClient actor, byte[] message)
        {
            // Validate connection
            if (!actor.Socket.Connected)
            {
                return;
            }

            // Read in TQ's binary header
            var length = BitConverter.ToUInt16(message, 0);
            var type = (PacketType)BitConverter.ToUInt16(message, 2);

            // Switch on the packet type
            MsgBase<LoginClient> msg;
            try
            {
                switch (type)
                {
                    case PacketType.MsgAccount: msg = new MsgAccount(); break;
                    case PacketType.MsgLoginProof: msg = new MsgLoginProof(); break;
                    case PacketType.MsgPCNum: msg = new MsgPCNum(); break;
                    case PacketType.MsgConnect:
                        {
                            // Conquer.exe 0x876ef7: after the reply 1055 the client reports res.dat here
                            return;
                        }
                    default:
                        {
                            // Conquer.exe has more login packets, not used by the normal login:
                            // 1084 CMsgConfirmKeyLogin and 1098 CMsgConfirmKeyLoginMobile (bind key),
                            // 1102 CMsgAccountSoftKb (soft keyboard), 1121 CMsgFacebookAccount (Facebook)
                            logger.Warning("Packet[{0}] Length[{1}] not handled by server, from {2}.", type, length, actor.IpAddress);
                            actor.Disconnect();
                            return;
                        }
                }

                msg.Decode(message);
                await msg.ProcessAsync(actor);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error on processing login message. {0}", ex.Message);
                actor.Disconnect();
            }
        }

        protected override void Disconnected(LoginClient actor)
        {
            UserManager.RemoveUser(actor.Guid);
        }
    }
}
