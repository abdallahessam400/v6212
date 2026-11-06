using Long.Login.Database.Entities;
using Long.Login.Database.Repositories;
using Long.Login.Managers;
using Long.Network.Packets;

namespace Long.Login.Network.Login.Packets
{
    /// <summary>
    ///     Conquer.exe 0x89a1dd, packet 1214 of 262 bytes: A at 5 with its length at 4, the proof at 134
    ///     with its length at 133.
    /// </summary>
    public sealed class MsgLoginProof : MsgBase<LoginClient>
    {
        private const int PUBLIC_LENGTH_OFFSET = 4;
        private const int PROOF_LENGTH_OFFSET = 133;

        private static readonly ILogger logger = Log.ForContext<MsgLoginProof>();

        public byte[] ClientPublic { get; private set; } = [];
        public byte[] Proof { get; private set; } = [];

        public override void Decode(byte[] bytes)
        {
            using var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();

            int publicLength = bytes[PUBLIC_LENGTH_OFFSET];
            int proofLength = bytes[PROOF_LENGTH_OFFSET];
            if (publicLength + PUBLIC_LENGTH_OFFSET + 1 > PROOF_LENGTH_OFFSET
                || proofLength + PROOF_LENGTH_OFFSET + 1 > bytes.Length)
            {
                return;
            }

            ClientPublic = bytes.AsSpan(PUBLIC_LENGTH_OFFSET + 1, publicLength).ToArray();
            Proof = bytes.AsSpan(PROOF_LENGTH_OFFSET + 1, proofLength).ToArray();
        }

        public override async Task ProcessAsync(LoginClient client)
        {
            GameAccount gameAccount = client.PendingAccount;
            Srp6 srp = client.Srp;
            string realm = client.PendingRealm;

            // one proof for each challenge
            client.PendingAccount = null;
            client.Srp = null;
            client.PendingRealm = null;

            if (gameAccount == null || srp == null)
            {
                logger.Warning("Login proof without a challenge from {0}.", client.IpAddress);
                await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.InvalidPassword);
                return;
            }

            if (!srp.Challenge(ClientPublic, Proof))
            {
                logger.Warning("User {0} has attempted to login with an invalid password.", gameAccount.UserName);
                LoginAttemptGuard.Fail(client.IpAddress, gameAccount.UserName);
                client.AccountID = (uint)gameAccount.Id;
                await LoginRecordRepository.LoginRcdAsync(client, null, false);
                await client.DisconnectWithRejectionCodeAsync(MsgConnectEx.RejectionCode.InvalidPassword);
                return;
            }

            LoginAttemptGuard.Success(client.IpAddress, gameAccount.UserName);
            await MsgAccount.CompleteLoginAsync(client, gameAccount, realm);
        }
    }
}
