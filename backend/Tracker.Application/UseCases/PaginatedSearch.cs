namespace Tracker.Application.UseCases;

public class PaginatedSearch
{
    public string? SearchQuery { get; set; }
    public required int Page { get; set; } = 1;
    public required int AmountInPage { get; set; } = 20;
}
