namespace Domain.Entities;

public class FileUpload : BaseEntity
{
    public required Guid UserId { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public required long SizeBytes { get; set; }
    public required string StorageFileName { get; set; }
    public required string StorageFolder { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

}
