using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Update;

public class UpdateBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardListCommand, Result>
{
    public async Task<Result> Handle(UpdateBoardListCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var listResult = await BoardHelper.GetBoardListForActionAsync(uow, userContext, request.BoardListId, BoardAction.ChangeList);
        if (listResult.IsFailure)
        {
            return listResult.Error;
        }
        var boardList = listResult.Value;

        boardList.Title = request.Title;
        boardList.Description = request.Description;

        uow.BoardListRepository.Update(boardList);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}