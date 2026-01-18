using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardItemCommand, Result>
{
    public async Task<Result> Handle(UpdateBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext, request.BoardItemId, BoardAction.ChangeItem);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var boardItem = itemResult.Value.Item1;

        boardItem.Title = request.Title;
        boardItem.Description = request.Description;

        uow.BoardItemRepository.Update(boardItem);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}