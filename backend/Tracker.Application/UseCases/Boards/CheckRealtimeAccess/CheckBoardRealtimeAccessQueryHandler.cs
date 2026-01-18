using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.CheckRealtimeAccess;

public class CheckBoardRealtimeAccessQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler< CheckBoardRealtimeAccessQuery , Result>
{
    public async Task<Result> Handle(
        CheckBoardRealtimeAccessQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdWithListsItemsUsersAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }
        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var userRole = user.Role;
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(user.Id, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(user.Id, board.Id);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden();
        }

        return Result.Success();
    }
}