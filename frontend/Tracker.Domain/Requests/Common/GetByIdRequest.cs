namespace Tracker.Domain.Requests.Common;

public class GetByIdRequest
{
    public required Guid Id { get; set; }

    public static implicit operator GetByIdRequest(Guid id)
    {
        return new GetByIdRequest { Id = id };
    }
}