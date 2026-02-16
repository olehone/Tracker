using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.ItemComments;

public class CommentGroup
{
    public UserDto User { get; }
    public Guid UserId => User.Id;
    public Guid Key { get; }
    public List<ItemCommentDto> Comments { get; }
    public CommentGroup(ItemCommentDto comment)
    {
        Comments = [comment];
        Key = Guid.NewGuid();
        User = comment.UploadedBy;
    }
}
