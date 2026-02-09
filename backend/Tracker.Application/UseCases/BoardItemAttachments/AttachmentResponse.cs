namespace Tracker.Application.UseCases.BoardItemAttachments;

public class AttachmentResponse
{
    public Stream? Stream { get; set; }
    public string? RedirectUrl { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
}
