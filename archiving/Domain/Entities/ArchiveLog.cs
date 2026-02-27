using Domain.Enums;

namespace Domain.Entities;

public class ArchiveLog
{
    public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;
    public ArchiveStatus? Status { get; set; }
    public required string Description { get; set; }
    public bool IsError { get; set; }
}
