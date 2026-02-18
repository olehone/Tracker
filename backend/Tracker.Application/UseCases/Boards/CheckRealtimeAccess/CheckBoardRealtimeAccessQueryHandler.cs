using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.CheckRealtimeAccess;

public class CheckBoardRealtimeAccessQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler< CheckBoardRealtimeAccessQuery , Result>
{
    public async Task<Result> Handle(
        CheckBoardRealtimeAccessQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdWithListsItemsUsersAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }
        return await BoardHelper.CanViewBoardAsync(board, uow, userContext, cancellationToken);
    }
}