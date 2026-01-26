using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
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

        if (request.Title is not null)
        {
            boardItem.Title = request.Title;
        }
        if (request.Description is not null)
        {
            boardItem.Description = request.Description;
        }
        if (request.IsDone is not null)
        {
            boardItem.IsDone = (bool)request.IsDone;
        }
        if (request.DueDate is not null)
        {
            boardItem.DueDate = request.DueDate;
        }
        if (request.Importance is not null)
        {
            boardItem.Importance = (BoardItemImportance)request.Importance;
        }

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