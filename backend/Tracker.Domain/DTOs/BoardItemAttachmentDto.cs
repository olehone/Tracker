namespace Tracker.Domain.Dtos;

public class BoardItemAttachmentDto
{
    public required Guid Id { get; set; }
    public required DateTimeOffset UploadedAt { get; set; }
    public required string UploadedByName { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required long SizeBytes { get; set; }
    public required bool IsDeleted { get; set; }
}
