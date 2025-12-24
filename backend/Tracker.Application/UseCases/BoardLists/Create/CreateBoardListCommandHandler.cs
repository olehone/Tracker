using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Create;

public sealed class CreateBoardListCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardListCommand, Result<BoardListDto>>
{
    public async Task<Result<BoardListDto>> Handle(
        CreateBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        int upperLimit = await uow.BoardListRepository.GetMaxPositionByBoardId(request.BoardId);

        var boardList = new BoardList()
        {
            BoardId = request.BoardId,
            Position = upperLimit + 1,
            Title = request.Title,
            Description = request.Description
        };
        await uow.BoardListRepository.AddAsync(boardList);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : boardList.ToDto();
    }
}
