using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Dtos;

public class BoardFullDto
{
    public required Guid Id { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required BoardVisibility Visibility { get; set; }
    public required BoardPermissionRoles PermissionRoles { get; set; }
    public required BoardPermissionsDto Permissions { get; set; }
    public required List<BoardListDto> BoardLists { get; set; }
    public required List<BoardUserDto> BoardUsers { get; set; }
}