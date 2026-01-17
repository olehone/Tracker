using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Delete;

public class DeleteBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<DeleteBoardListCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardListForActionAsync(uow, userContext, request.BoardListId, BoardAction.ChangeList);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }
        var boardList = listResult.Value;

        await uow.BoardListRepository.RemoveAsync(boardList.Id);
        
        var maxPosition = await uow.BoardListRepository.GetMaxPositionByBoardId(boardList.BoardId);
        await uow.BoardListRepository.ShiftPositions(
            boardList.BoardId, -1, boardList.Position + 1, maxPosition);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}