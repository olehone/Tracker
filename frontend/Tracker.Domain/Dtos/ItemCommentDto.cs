namespace Tracker.Domain.Dtos;

public class ItemCommentDto
{
    public required Guid Id { get; set; }
    public required Guid ItemId { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset UploadedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required UserDto UploadedBy { get; set; }
    public required ICollection<FileDto> Attachments { get; set; }
}