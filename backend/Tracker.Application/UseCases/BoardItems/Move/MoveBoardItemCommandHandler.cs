using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Move;

public class MoveBoardItemCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<MoveBoardItemCommand, Result>
{
    public async Task<Result> Handle(
        MoveBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boardItem = await uow.BoardItemRepository.GetByIdAsync(request.BoardItemId);
        if (boardItem is null)
        {
            return Error.NotFound("BoardItem");
        }

        int currentPosition = boardItem.Position;
        int maxNewPosition = await uow.BoardItemRepository
            .GetMaxPositionByListIdAsync(request.ToBoardListId) + 1;

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
            await uow.BoardItemRepository.ShiftPositions(
                    boardItem.BoardListId, -1, currentPosition + 1);
            await uow.BoardItemRepository.ShiftPositions(
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
