namespace Tracker.Domain.Requests;

public class CursorTimeRequest
{
    public required int Amount { get; set; } = 20;
    public DateTimeOffset? Before { get; set; } = DateTimeOffset.UtcNow;
}
