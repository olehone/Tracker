namespace Tracker.Domain.Dtos;

public class ItemCommentDto
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}