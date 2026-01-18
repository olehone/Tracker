using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Delete;

public class DeleteBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<DeleteBoardItemCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext, 
            request.BoardItemId, BoardAction.ChangeItem, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var boardItem = itemResult.Value;

        await uow.BoardItemRepository.RemoveAsync(boardItem.Id);
        await uow.BoardItemRepository.ShiftPositionsAsync(
            boardItem.BoardListId, -1, boardItem.Position);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}