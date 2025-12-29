namespace Tracker.Domain.Requests.Common;

public class SearchByFieldPartRequest
{
    public required string SearchQuery { get; set; }
    public required int Page { get; set; } = 1;
    public required int AmountInPage { get; set; } = 20;
}
