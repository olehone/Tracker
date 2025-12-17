using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Entities.Commands;

public class LoginUserCommand
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
