using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class BoardUserRoleRequest
{
    public required UserBoardRole Role { get; set; }

    public static implicit operator BoardUserRoleRequest(UserBoardRole role)
    {
        return new BoardUserRoleRequest { Role = role };
    }
}