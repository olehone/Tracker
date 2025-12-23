using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetBoardById;

public class GetBoardByIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetBoardByIdQuery, Result<BoardFullDto>>
{
    public async Task<Result<BoardFullDto>> Handle(
        GetBoardByIdQuery query, 
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdWithListsAndItemsAsync(query.Id);

        if (board == null)
        {
            return new Error(
                "Board.NotFound",
                ErrorType.NotFound,
                "Board with this id is not found");
        }
        return board.ToFullDto();
    }
}
