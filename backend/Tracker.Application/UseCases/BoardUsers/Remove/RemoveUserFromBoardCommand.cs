using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Remove;

public class RemoveUserFromBoardCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public required Guid UserId { get; set; }
}