using Tracker.Domain.Entities.Common;
using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Username { get; set; }
    public required GlobalRole Role { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }

    public List<UserBoard> UserBoards { get; set; } = [];
    public List<UserWorkspace> UserWorkspaces { get; set; } = [];
}
