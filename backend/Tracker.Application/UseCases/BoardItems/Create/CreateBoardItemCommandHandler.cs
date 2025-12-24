using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Create;

public class CreateBoardItemCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardItemCommand, Result<BoardItemDto>>
{
    public async Task<Result<BoardItemDto>> Handle(
        CreateBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boardList = await uow.BoardListRepository.GetByIdAsync(request.BoardListId);
        if (boardList is null)
        {
            return Error.NotFound("Board");
        }
        int upperLimit = await uow.BoardItemRepository
            .GetMaxPositionByListIdAsync(request.BoardListId);

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
