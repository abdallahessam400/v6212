using Long.Network.Packets;

namespace Long.Login.Network.Login.Packets
{
    /// <summary>
    ///     Answer of the server to packet 1542. Conquer.exe has no password in that packet, it proves the
    ///     password with SRP-6: B at 5, the salt at 392, each one with its length in the byte before it.
    /// </summary>
    public sealed class MsgSrpChallenge : MsgBase<LoginClient>
    {
        private const int PACKET_SIZE = 424;
        private const int PUBLIC_OFFSET = 4;
        private const int SALT_OFFSET = 391;

        public MsgSrpChallenge(byte[] serverPublic, byte[] salt)
        {
            ServerPublic = serverPublic;
            Salt = salt;
        }

        public byte[] ServerPublic { get; }
        public byte[] Salt { get; }

        public override byte[] Encode()
        {
            byte[] body = new byte[PACKET_SIZE - 4];
            body[PUBLIC_OFFSET - 4] = (byte)ServerPublic.Length;
            ServerPublic.CopyTo(body, PUBLIC_OFFSET - 3);
            body[SALT_OFFSET - 4] = (byte)Salt.Length;
            Salt.CopyTo(body, SALT_OFFSET - 3);

            using var writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgSrpChallenge);
            writer.Write(body);
            return writer.ToArray();
        }
    }
}
