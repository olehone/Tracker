namespace Tracker.API.Requests;

public class CursorTimeRequest
{
    public required DateTimeOffset Before { get; set; } = DateTimeOffset.UtcNow;
    public required int Amount { get; set; } = 20;
}
