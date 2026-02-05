namespace Tracker.Domain.Entities;

public class BoardItemAttachment : FileUpload
{
    public required Guid BoardItemId { get; set; }
    public BoardItem Item { get; set; } = null!;
    
}
