using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class BoardUserRoleRequest
{
    public required BoardUserRole Role { get; set; }
}