using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.BoardUser;

public class BoardUserRoleRequest
{
    public required BoardUserRole Role { get; set; }

    public static implicit operator BoardUserRoleRequest(BoardUserRole role)
    {
        return new BoardUserRoleRequest { Role = role };
    }
}