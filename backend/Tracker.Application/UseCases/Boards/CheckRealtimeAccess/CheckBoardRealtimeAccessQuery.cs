using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.CheckRealtimeAccess;

public class CheckBoardRealtimeAccessQuery : IRequest<Result>
{
    public required Guid BoardId { get; set; }
}