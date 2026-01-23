namespace Tracker.Domain.Dtos;

public class BoardItemDto
{
    public required Guid Id { get; set; }
    public required Guid BoardListId { get; set; }
    public int Position { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsDone { get; set; }
    public required HashSet<Guid> Assignees { get; set; }
}