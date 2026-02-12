using Tracker.Domain.Entities.Common;
using Tracker.Domain.Results;

namespace Tracker.Domain.Entities;

public class ItemComment : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public User UploadedBy { get; set; } = null!;
    //public ICollection<BoardItemAttachment> Attachments { get; set; } = [];
}
