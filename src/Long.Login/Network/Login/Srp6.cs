using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Long.Login.Network.Login;

/// <summary>
///     The password proof of Conquer.exe: N at 0xa44550, the password suffix at 0xa4452e.
/// </summary>
public sealed class Srp6
{
    private const int CredentialBytes = 32;

    private static readonly LegacyBigInt N = LegacyBigInt.FromHexString(
        "894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7");
    private static readonly LegacyBigInt G = LegacyBigInt.FromHexString("07");
    private static readonly LegacyBigInt K = LegacyBigInt.FromHexString("03");

    // a new one for each login, with a fixed value a captured proof could be sent again
    private readonly LegacyBigInt privateB = LegacyBigInt.FromMemoryBytes(RandomNumberGenerator.GetBytes(CredentialBytes));
    private readonly LegacyBigInt salt;
    private readonly byte[] saltBytes;
    private readonly LegacyBigInt verifier;
    private readonly LegacyBigInt serverPublic;
    private readonly LegacyBigInt identity;

    public Srp6(string username, string verifierHex, string saltHex)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        saltBytes = ParseFixedHex(saltHex, CredentialBytes, nameof(saltHex));
        byte[] verifierBytes = ParseFixedHex(verifierHex, CredentialBytes, nameof(verifierHex));
        salt = LegacyBigInt.FromMemoryBytes(saltBytes);
        verifier = LegacyBigInt.FromMemoryBytes(verifierBytes);
        identity = LegacyBigInt.FromMemoryBytes(
            Encoding.ASCII.GetBytes(username + (username.Length % 2 == 0 ? string.Empty : "\0")));
        serverPublic = BuildServerPublic(verifier);
    }

    public static string CreateSaltHex()
    {
        byte[] value;
        do
        {
            value = RandomNumberGenerator.GetBytes(CredentialBytes);
        }
        while (value[^1] == 0);

        return Convert.ToHexString(value).ToLowerInvariant();
    }

    public static string CreateVerifierHex(string plainPassword, string saltHex)
    {
        byte[] rawSalt = ParseFixedHex(saltHex, CredentialBytes, nameof(saltHex));
        LegacyBigInt salt = LegacyBigInt.FromMemoryBytes(rawSalt);
        LegacyBigInt password = ComputePassword(plainPassword ?? string.Empty);
        LegacyBigInt x = HashBigInt(salt, password);
        LegacyBigInt verifier = G.Duplicate();
        verifier.ModPow(x, N);
        return Convert.ToHexString(verifier.GetBytes(CredentialBytes)).ToLowerInvariant();
    }

    public void GetOutgoingFields(out byte[] serverPublicBytes, out byte[] outgoingSaltBytes)
    {
        serverPublicBytes = serverPublic.GetBytes();
        outgoingSaltBytes = saltBytes.ToArray();
    }

    public bool Challenge(byte[] clientPublicBytes, byte[] clientProof)
    {
        if (clientPublicBytes == null || clientPublicBytes.Length == 0 ||
            clientProof == null || clientProof.Length == 0)
        {
            return false;
        }

        LegacyBigInt clientPublic = LegacyBigInt.FromMemoryBytes(clientPublicBytes);

        // A = 0 mod N makes the session 0 without the password
        if (clientPublic.IsMultipleOf(N))
        {
            return false;
        }

        LegacyBigInt u = HashBigInt(clientPublic, serverPublic);
        LegacyBigInt session = verifier.Duplicate();
        session.ModPow(u, N);
        session.Multiply(clientPublic);
        session.ModPow(privateB, N);
        LegacyBigInt interleaved = ShaInterleave(session);

        byte[] expected = Hash(
            XorNgHashes(),
            Hash(identity),
            salt,
            clientPublic,
            serverPublic,
            interleaved);

        return expected.Length == clientProof.Length &&
               CryptographicOperations.FixedTimeEquals(expected, clientProof);
    }

    private static byte[] ParseFixedHex(string value, int expectedBytes, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != expectedBytes * 2)
        {
            throw new ArgumentException($"{parameterName} must contain exactly {expectedBytes * 2} hexadecimal characters.", parameterName);
        }

        try
        {
            return Convert.FromHexString(value);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"{parameterName} is not valid hexadecimal.", parameterName, ex);
        }
    }

    private static LegacyBigInt ComputePassword(string plainPassword)
    {
        byte[] suffix =
        [
            163, 172, 161, 163, 102,
            100, 106, 102, 44, 106,
            107, 103, 102, 107, 108
        ];
        byte[] passwordBytes = Encoding.ASCII.GetBytes(plainPassword);
        byte[] input = new byte[passwordBytes.Length + suffix.Length];
        passwordBytes.CopyTo(input, 0);
        suffix.CopyTo(input, passwordBytes.Length);
        byte[] md5 = MD5.HashData(input);
        return LegacyBigInt.FromMemoryHexString("00000000" + Convert.ToHexString(md5));
    }

    private LegacyBigInt BuildServerPublic(LegacyBigInt value)
    {
        LegacyBigInt result = K.Duplicate();
        result.Multiply(value);
        LegacyBigInt gb = G.Duplicate();
        gb.ModPow(privateB, N);
        result.Add(gb);
        return result;
    }

    private static LegacyBigInt HashBigInt(params object[] values)
        => LegacyBigInt.FromMemoryBytes(Hash(values));

    private static byte[] Hash(params object[] values)
    {
        using SHA1 sha = SHA1.Create();
        using MemoryStream stream = new();
        foreach (object value in values)
        {
            switch (value)
            {
                case LegacyBigInt bigInt:
                    stream.Write(bigInt.GetBytes());
                    break;
                case byte[] bytes:
                    stream.Write(bytes);
                    break;
            }
        }
        return sha.ComputeHash(stream.ToArray());
    }

    private static LegacyBigInt ShaInterleave(LegacyBigInt session)
    {
        byte[] bytes = session.GetBytes();
        int half = bytes.Length / 2;
        byte[] even = new byte[half];
        byte[] odd = new byte[half];
        for (int index = 0; index < half; index++)
        {
            even[index] = bytes[index * 2];
            odd[index] = bytes[index * 2 + 1];
        }

        byte[] evenHash = SHA1.HashData(even);
        byte[] oddHash = SHA1.HashData(odd);
        byte[] output = new byte[40];
        for (int index = 0; index < 20; index++)
        {
            output[index * 2] = evenHash[index];
            output[index * 2 + 1] = oddHash[index];
        }
        return LegacyBigInt.FromMemoryBytes(output);
    }

    private static byte[] XorNgHashes()
    {
        byte[] nHash = Hash(N);
        byte[] gHash = Hash(G);
        for (int index = 0; index < nHash.Length; index++)
        {
            nHash[index] ^= gHash[index];
        }
        return nHash;
    }

    private sealed class LegacyBigInt
    {
        private BigInteger Value { get; set; }

        private LegacyBigInt(BigInteger value)
        {
            Value = value;
        }

        public static LegacyBigInt FromHexString(string value)
            => new(new BigInteger(Convert.FromHexString(value), isUnsigned: true, isBigEndian: true));

        public static LegacyBigInt FromMemoryHexString(string value)
        {
            byte[] bytes = Convert.FromHexString(value);
            Array.Reverse(bytes);
            return FromHexString(Convert.ToHexString(bytes));
        }

        public static LegacyBigInt FromMemoryBytes(byte[] value)
            => new(new BigInteger(value, isUnsigned: true, isBigEndian: false));

        public LegacyBigInt Duplicate() => new(Value);
        public bool IsMultipleOf(LegacyBigInt value) => (Value % value.Value).IsZero;
        public void Add(LegacyBigInt value) => Value += value.Value;
        public void Multiply(LegacyBigInt value) => Value *= value.Value;
        public void ModPow(LegacyBigInt exponent, LegacyBigInt modulus)
            => Value = BigInteger.ModPow(Value, exponent.Value, modulus.Value);

        public byte[] GetBytes()
        {
            byte[] bytes = Value.ToByteArray(isUnsigned: true, isBigEndian: false);
            return bytes.Length == 0 ? [0] : bytes;
        }

        public byte[] GetBytes(int fixedLength)
        {
            byte[] bytes = GetBytes();
            if (bytes.Length > fixedLength)
            {
                throw new InvalidOperationException($"Big integer does not fit in {fixedLength} bytes.");
            }

            byte[] output = new byte[fixedLength];
            bytes.CopyTo(output, 0);
            return output;
        }
    }
}
