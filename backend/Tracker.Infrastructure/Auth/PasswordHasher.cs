using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Auth;

internal class PasswordHasher(IOptions<PasswordHasherOptions> options) : IPasswordHasher
{
    public string Hash(string password)
    {

        byte[] salt = RandomNumberGenerator.GetBytes(options.Value.HashSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,
            salt,
            options.Value.Iterations,
            options.Value.AlgorithmName,
            options.Value.HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split("-");
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password,
            salt,
            options.Value.Iterations,
            options.Value.AlgorithmName,
            options.Value.HashSize
        );

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}
