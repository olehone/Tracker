using Tracker.Domain.Entities;

namespace Tracker.Domain.Dtos;

public class BoardListDto
{
    public required Guid Id { get; set; }
    public int Position { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<BoardItemDto> BoardItems { get; set; } = [];
}
