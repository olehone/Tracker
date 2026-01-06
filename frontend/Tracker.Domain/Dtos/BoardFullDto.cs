using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class BoardFullDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required BoardVisibility Visibility { get; set; }
    public required BoardPermissionsDto Permissions { get; set; }
    public required List<BoardListDto> BoardLists { get; set; }
}