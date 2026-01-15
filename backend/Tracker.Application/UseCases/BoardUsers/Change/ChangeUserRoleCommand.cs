using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Change;

public class ChangeUserRoleCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public required Guid UserId { get; set; }
    public required UserBoardRole Role { get; set; }
}