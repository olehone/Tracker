namespace Tracker.API.Requests;

public class FileUploadRequest
{
    public required IFormFile File { get; set; }
}
