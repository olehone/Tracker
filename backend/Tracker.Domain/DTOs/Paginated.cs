namespace Tracker.Domain.Dtos;

public class Paginated<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public required int TotalCount { get; set; }

    public static Paginated<T> Empty() => new()
    {
        Items = [],
        TotalCount = 0
    };
}