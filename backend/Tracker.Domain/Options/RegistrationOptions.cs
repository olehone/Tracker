using System.ComponentModel.DataAnnotations;
using Tracker.Domain.Entities;

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
    [Required]
    public string DefaultUserRole { get; init; } = Roles.User;
}
