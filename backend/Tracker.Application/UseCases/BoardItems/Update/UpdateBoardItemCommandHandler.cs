using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardItemCommand, Result<BoardItemDto>>
{
    public async Task<Result<BoardItemDto>> Handle(UpdateBoardItemCommand request,
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

        boardItem.Title = request.Title;
        boardItem.Description = request.Description;

        uow.BoardItemRepository.Update(boardItem);
        var result = await uow.SaveChangesAsync(cancellationToken);

        var item = await uow.BoardItemRepository.GetByIdAsync(request.BoardItemId);
        if (result.IsFailure || item is null)
        {
            return Error.Unknown;
        }
        return item.ToDto();
    }
}