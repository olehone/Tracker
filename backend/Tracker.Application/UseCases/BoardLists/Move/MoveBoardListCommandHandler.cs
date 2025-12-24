using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Move;

public sealed class MoveBoardListCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<MoveBoardListCommand, Result>
{
    public async Task<Result> Handle(
        MoveBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boardList = await uow.BoardListRepository.GetByIdAsync(request.BoardListId);
        if (boardList is null)
        {
            return Error.NotFound("BoardList");
        }

        int currentPosition = boardList.Position;
        if (currentPosition == request.Position)
        {
            return Result.Success();
        }

        var board = await uow.BoardRepository.GetByIdAsync(boardList.BoardId);
        if(board is null)
        {
            return Error.NotFound("Board", "board list");
        }

        int maxNewPosition = await uow.BoardListRepository.GetMaxPositionByBoardId(board.Id) + 1;
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
