using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Add;

public class AddUserToBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<AddUserToBoardCommand, Result<BoardUserDto>>
{
    public async Task<Result<BoardUserDto>> Handle(
        AddUserToBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        var user = await uow.UserRepository.GetByIdAsync(request.BoardId);
        if (user is null)
        {
            return Error.NotFound("User");
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
        var boardUser = new UserBoard
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role,
        };
        await uow.UserBoardRepository.AddAsync(boardUser);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        var dto = new BoardUserDto
        {
            User = user.ToDto(),
            Role = request.Role
        };

        return sc.IsFailure
            ? Error.Unknown
            : dto;
    }
}