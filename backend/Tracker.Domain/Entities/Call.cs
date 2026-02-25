namespace Tracker.Domain.Entities;

public class Call
{
    public required Guid Id { get; set; }
    public required DateTimeOffset StartedAt { get; set; }
    public required List<CallUser> Users { get; set; }
}