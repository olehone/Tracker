using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Move;

public class MoveBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<MoveBoardItemCommand, Result>
{
    public async Task<Result> Handle(
        MoveBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext, request.BoardItemId, BoardAction.ChangeItem);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }
        var boardItem = listResult.Value;

        int currentPosition = boardItem.Position;
        int maxNewPosition = await uow.BoardItemRepository
            .GetMaxPositionAsync(request.ToBoardListId) + 1;

        if (maxNewPosition < request.Position)
        {
            request.Position = maxNewPosition;
        }

        if (boardItem.BoardListId == request.ToBoardListId)
        {
            if (currentPosition == request.Position)
            {
                return Result.Success();
            }
            else if (currentPosition < request.Position)
            {
                await uow.BoardItemRepository.ShiftPositions(
                    boardItem.BoardListId, -1, currentPosition + 1, request.Position);
            }
            else
            {
                await uow.BoardItemRepository.ShiftPositions(
                    boardItem.BoardListId, +1, request.Position, currentPosition - 1);
            }
        }
        else
        {
            await uow.BoardItemRepository.ShiftPositionsAsync(
                    boardItem.BoardListId, -1, currentPosition + 1);
            await uow.BoardItemRepository.ShiftPositionsAsync(
                    request.ToBoardListId, +1, request.Position);

            boardItem.BoardListId = request.ToBoardListId;
        }

        boardItem.Position = request.Position;
        uow.BoardItemRepository.Update(boardItem);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}
