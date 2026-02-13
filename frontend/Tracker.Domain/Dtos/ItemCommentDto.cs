namespace Tracker.Domain.Dtos;

public class ItemCommentDto
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset UploadedAt { get; set; }
    public required ICollection<FileDto> Attachments { get; set; }
}