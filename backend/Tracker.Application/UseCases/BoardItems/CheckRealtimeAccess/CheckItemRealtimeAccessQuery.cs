using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.CheckRealtimeAccess;

public class CheckItemRealtimeAccessQuery : IRequest<Result>
{
    public required Guid ItemId { get; set; }
    public required Guid UserId { get; set; }
}