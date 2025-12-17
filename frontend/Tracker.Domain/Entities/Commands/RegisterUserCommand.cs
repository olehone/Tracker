using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Entities.Commands;
public class RegisterUserCommand
{
    [Required]
    public string Email {get; set;} = string.Empty;
    [Required]
    public string Password {get; set;} = string.Empty;
    [Required]
    public string Username {get; set;} = string.Empty;
    [Required]
    public string FirstName {get; set;} = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
