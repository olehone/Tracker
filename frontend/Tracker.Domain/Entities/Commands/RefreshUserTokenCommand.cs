using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Entities.Commands;

public class RefreshUserTokenCommand
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
