using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
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

        var board = await BoardHelper.GetBoardForActionAsync(uow, userContext,
            request.BoardId, BoardAction.ChangeBoard);
        if (board.IsFailure)
        {
            return board.Error;
        }

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var userBoard = await uow.BoardUserRepository
            .GetAsync(request.UserId, request.BoardId);
        if (userBoard is not null)
        {
            return Error.AlreadyExists("User", "Board", user.Username);
        }

        var boardUser = new BoardUser
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role,
        };
        await uow.BoardUserRepository.AddAsync(boardUser);

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