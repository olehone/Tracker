using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.BoardUsers.Change;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Remove;

public class RemoveUserFromBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<RemoveUserFromBoardCommand, Result>
{
    public async Task<Result> Handle(
        RemoveUserFromBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext,
            request.BoardId, BoardAction.ChangeBoard);
        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }
        var board = boardResult.Value;

        var userBoard = await uow.BoardUserRepository
            .GetAsync(request.UserId, request.BoardId);
        if (userBoard is null)
        {
            return Error.NotFound("User", "board");
        }
        var userId = userContext.GetUserId();

        if (userBoard.Role == BoardUserRole.Owner && userBoard.UserId == userId)
        {
            await uow.BoardRepository.RemoveAsync(board.Id);
        }
        else
        {
            await uow.BoardUserRepository.RemoveAsync(userBoard.Id);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

}