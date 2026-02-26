namespace Domain.Entities;

public class BoardItemAssignee : BaseEntity
{
    public required Guid BoardUserId { get; set; }
    public required Guid BoardItemId { get; set; }
}
