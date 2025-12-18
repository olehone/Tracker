using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Tracker.Domain.Options;

public class PasswordHasherOptions
{
    public const string SectionName = "PasswordHasherOptions";
    public required int SaltSize { get; init; }
    public required int HashSize { get; init; }
    public required int Iterations { get; init; }
    public required string Algorithm { get; init; }
    public HashAlgorithmName AlgorithmName =>
        new (Algorithm);
}
