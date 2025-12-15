using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Tracker.Domain.Options;

public class PasswordHasherOptions
{
    public const string SectionName = "PasswordHasherOptions";
    [Required]
    public int SaltSize { get; init; }
    [Required]
    public int HashSize { get; init; }
    [Required]
    public int Iterations { get; init; }
    [Required]
    public string Algorithm { get; init; } = default!;
    public HashAlgorithmName AlgorithmName =>
        new HashAlgorithmName(Algorithm);
}
