namespace Tracker.Domain.Entities;

public class CommentAttachment : FileUpload
{
    public required Guid BoardCommentId { get; set; }
    public ItemComment Comment { get; set; } = null!;

}
