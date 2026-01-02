namespace Tracker.Domain.Dtos;

public class BoardFullDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<BoardListDto> BoardLists { get; set; } = [];
}
