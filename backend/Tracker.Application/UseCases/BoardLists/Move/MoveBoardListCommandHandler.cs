using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Move;

public sealed class MoveBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<MoveBoardListCommand, Result>
{
    public async Task<Result> Handle(
        MoveBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardListForActionAsync(uow, userContext, request.BoardListId, BoardAction.ChangeList);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }
        var boardList = listResult.Value;

        int currentPosition = boardList.Position;
        if (currentPosition == request.Position)
        {
            return Result.Success();
        }

        int maxNewPosition = await uow.BoardListRepository.GetMaxPositionByBoardId(boardList.Id) + 1;
        if (maxNewPosition < request.Position)
        {
            request.Position = maxNewPosition;
        }

        if (currentPosition < request.Position)
        {
            await uow.BoardListRepository.ShiftPositions(
                boardList.BoardId, -1, currentPosition + 1, request.Position);
        }
        else
        {
            await uow.BoardListRepository.ShiftPositions(
                boardList.BoardId, +1, request.Position, currentPosition - 1);
        }

        boardList.Position = request.Position;
        uow.BoardListRepository.Update(boardList);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}
