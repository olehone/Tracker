namespace Domain.Entities;

public class BoardMetadata
{
    public required Guid BoardId { get; set; }
    public required ArchiveLog LastLog { get; set; }
    public List<ArchiveLog> Logs { get; set; } = [];
}