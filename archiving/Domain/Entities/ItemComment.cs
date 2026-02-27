namespace Domain.Entities;

public class ItemComment : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ICollection<CommentAttachment> Attachments { get; set; } = [];
}