using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Get;

public class GetUsersByBoardIdQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUsersByBoardIdQuery, Result<List<BoardUserDto>>>
{
    public async Task<Result<List<BoardUserDto>>> Handle(
        GetUsersByBoardIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(userId, board.Id);

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);


        if (!BoardPolicy.IsActionAllowed(permissions, BoardAction.ChangeBoard))
        {
            return AuthErrors.Forbidden();
        }

        var boardUsers = await uow.UserBoardRepository.GetByBoardAsync(board.Id);

        return boardUsers.Select(bm => new BoardUserDto
        {
            User = bm.User.ToDto(),
            Role = bm.Role,
        }).ToList();
    }
}