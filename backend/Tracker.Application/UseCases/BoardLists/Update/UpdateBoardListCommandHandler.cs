using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Update;

public class UpdateBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardListCommand, Result<BoardListDto>>
{
    public async Task<Result<BoardListDto>> Handle(UpdateBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardListForActionAsync(uow, userContext,
            request.BoardListId, BoardAction.ChangeList, request.BoardId);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }
        var boardList = listResult.Value;

        boardList.Title = request.Title;
        boardList.Description = request.Description;

        uow.BoardListRepository.Update(boardList);
        var result = await uow.SaveChangesAsync(cancellationToken);
        
        var list = await uow.BoardListRepository.GetByIdAsync(request.BoardListId);
        if (result.IsFailure || list is null)
        {
            return Error.Unknown;
        }
        return list.ToDto();
    }
}