namespace Tracker.Domain.Dtos;

public class CursorPage<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public DateTimeOffset? LastLoadedAt { get; set; }
    public required bool HasMore { get; set; }

    public static CursorPage<T> Empty() => new()
    {
        Items = [],
        HasMore = false,
    };
}