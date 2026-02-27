using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.States;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetById;

public class GetBoardByIdQueryHandler(
    IBoardCallState boardCallRepo,
    ICallState callRepo,
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetBoardByIdQuery, Result<BoardFullDto>>
{
    public async Task<Result<BoardFullDto>> Handle(
        GetBoardByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdWithListsItemsUsersAsync(request.Id);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (userContext.IsUnauthenticated())
        {
            return BoardPolicy.CanAnonView(board.Visibility)
                ? (Result<BoardFullDto>)board.ToFullDto(BoardPermissionsDto.None)
                : (Result<BoardFullDto>)AuthErrors.Forbidden("Board is private");
        }

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        var userRole = user.Role;

        var workspaceRole = await uow.WorkspaceUserRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.BoardUserRepository
            .GetRoleAsync(userId, board.Id);

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden("Board is private");
        }

        if (BoardHelper.IsArchiveStatusBlocking(board))
        {
            return Error.Archived("Board");
        }

        var boardDto = board.ToFullDto(permissions);
        var callId = await boardCallRepo.GetCallIdAsync(board.Id);
        if (callId is null)
        {
            return boardDto;
        }

        var call = await callRepo.GetCallByIdAsync(callId.Value);

        if (call is null)
        {
            await boardCallRepo.RemoveCallAsync(board.Id);
        }
        else
        {
            boardDto.CallId = callId;
        }

        return boardDto;
    }
}
