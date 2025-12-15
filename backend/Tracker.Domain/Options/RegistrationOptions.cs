using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class RegistrationOptions
{
    public const string SectionName = "RegistrationOptions";
    [Required]
    public int PasswordMinimumLength { get; init; }
    [Required]
    public int UsernameMinimumLength { get; init; }
    [Required]
    public int UsernameMaximumLength { get; init; }
}
