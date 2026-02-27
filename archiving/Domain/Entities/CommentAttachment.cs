namespace Domain.Entities;

public class CommentAttachment : FileUpload
{
    public required Guid ItemCommentId { get; set; }
}