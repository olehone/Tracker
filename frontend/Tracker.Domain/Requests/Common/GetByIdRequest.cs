namespace Tracker.Domain.Requests.Common;

public class GetByIdRequest
{
    public required Guid Id { get; set; }
}