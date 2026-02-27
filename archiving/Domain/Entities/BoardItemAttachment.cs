namespace Domain.Entities;

public class BoardItemAttachment : FileUpload
{
    public required Guid BoardItemId { get; set; }
}
