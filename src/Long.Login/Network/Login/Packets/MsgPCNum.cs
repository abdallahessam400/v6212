using Long.Login.Database.Repositories;
using Long.Network.Packets;

namespace Long.Login.Network.Login.Packets
{
    /// <summary>
    ///     Conquer.exe 0x876e5d: packet 1100 of 52 bytes (0x89952b), sent here right after the reply 1055. The
    ///     MAC is at 8 as 12 hex letters. At 4 the client repeats the first half of our token, not the account.
    /// </summary>
    public sealed class MsgPCNum : MsgBase<LoginClient>
    {
        private const int MAC_LENGTH = 12;

        public string MacAddress { get; private set; } = string.Empty;

        public override void Decode(byte[] bytes)
        {
            using var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            reader.BaseStream.Seek(8, SeekOrigin.Begin);
            MacAddress = reader.ReadString(MAC_LENGTH);
        }

        public override Task ProcessAsync(LoginClient client)
        {
            // once, and only for the account that logged in on this connection
            if (client.AccountID != 0 && !client.MacSaved
                && MacAddress.Length == MAC_LENGTH && MacAddress.All(Uri.IsHexDigit))
            {
                client.MacSaved = true;
                AccountRepository.SetMacAddress((int)client.AccountID, MacAddress);
            }

            return Task.CompletedTask;
        }
    }
}
