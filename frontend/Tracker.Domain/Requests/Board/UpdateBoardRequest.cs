using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Requests.Board;

public class UpdateBoardRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required BoardVisibility Visibility { get; set; }
    public required BoardPermissionRoles PermissionRoles { get; set; }
}
