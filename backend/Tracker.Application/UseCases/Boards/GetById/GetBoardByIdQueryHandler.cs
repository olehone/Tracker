using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetById;

public class GetBoardByIdQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetBoardByIdQuery, Result<BoardFullDto>>
{
    public async Task<Result<BoardFullDto>> Handle(
        GetBoardByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdWithListsAndItemsAsync(request.Id);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (!userContext.IsAuthenticated())
        {
            if (BoardPolicy.CanAnonView(board.Visibility))
            {
                return board.ToFullDto(BoardPermissionsDto.None);
            }
            return AuthErrors.Forbidden();
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(userId, board.Id);

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden();
        }
        return board.ToFullDto(permissions);
    }
}
