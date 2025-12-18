using System.ComponentModel.DataAnnotations;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Options;

public class RegistrationOptions
{
    public const string SectionName = "RegistrationOptions";
    public required int PasswordMinimumLength { get; init; }
    public required int UsernameMinimumLength { get; init; }
    public required int UsernameMaximumLength { get; init; }
    public string DefaultUserRole { get; init; } = Roles.User;
}
