namespace Tracker.API.Requests;

public class CreateCommentRequest
{
    public required string Content { get; set; }
    public required List<IFormFile> Files { get; set; }
}
