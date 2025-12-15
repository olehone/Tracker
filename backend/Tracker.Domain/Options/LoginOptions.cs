using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class LoginOptions
{
    public const string SectionName = "LoginOptions";
    
    [Required]
    public int PasswordMinimumLength { get; init; }
}
