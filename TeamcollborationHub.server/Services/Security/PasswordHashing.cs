using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace TeamcollborationHub.server.Services.Security;

public class PasswordHashing: IPasswordHashingService
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            _hashAlgorithm,
            HashSize
        );
        return $"{Convert.ToHexString(salt)}-{Convert.ToHexString(hash)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        byte[] hash= Convert.FromHexString(parts[1]);
        byte[] salt = Convert.FromHexString(parts[0]);

        byte[] hashToVerify = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            _hashAlgorithm,
            HashSize
        ); 

        return CryptographicOperations.FixedTimeEquals(hash, hashToVerify);
    }
}
