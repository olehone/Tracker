using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Create;

public sealed class CreateBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardListCommand, Result<BoardListDto>>
{
    public async Task<Result<BoardListDto>> Handle(
        CreateBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext, 
            request.BoardId, BoardAction.CreateList);

        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }

        int upperLimit = await uow.BoardListRepository.GetMaxPositionAsync(request.BoardId);

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