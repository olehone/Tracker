namespace Tracker.Domain.Dtos;

public class BoardListDto
{
    public required Guid Id { get; set; }
    public int Position { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required List<BoardItemDto> BoardItems { get; set; }
}