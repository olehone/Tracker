namespace ArchivingFunction.Domain.Entities;

public class CommentAttachment : FileUpload
{
    public required Guid ItemCommentId { get; set; }
    public ItemComment Comment { get; set; } = null!;
}