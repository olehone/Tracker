using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Create;

public class CreateBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardItemCommand, Result<BoardItemDto>>
{
    public async Task<Result<BoardItemDto>> Handle(
        CreateBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardListForActionAsync(uow, userContext, request.BoardListId, BoardAction.CreateItem);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }

        int upperLimit = await uow.BoardItemRepository
            .GetMaxPositionAsync(request.BoardListId);

        var boardItem = new BoardItem()
        {
            BoardListId = request.BoardListId,
            Position = upperLimit + 1,
            Title = request.Title,
            Description = request.Description
        };
        await uow.BoardItemRepository.AddAsync(boardItem);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : boardItem.ToDto();
    }
}
