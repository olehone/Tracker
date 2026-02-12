namespace Tracker.API.Requests;

public class PaginatedTimeRequest
{
    public required DateTimeOffset Before { get; set; } = DateTimeOffset.UtcNow;
    public required Guid LastEntity { get; set; }
    public required int AmountInPage { get; set; } = 20;
}
