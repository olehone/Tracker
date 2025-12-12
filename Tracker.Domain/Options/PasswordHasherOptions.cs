using System.ComponentModel.DataAnnotations;

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
}
