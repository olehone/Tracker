using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.CheckRealtimeAccess;

public class CheckItemRealtimeAccessQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext
    )
    : IRequestHandler<CheckItemRealtimeAccessQuery, Result>
{
    public async Task<Result> Handle(
        CheckItemRealtimeAccessQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var item = await BoardHelper.GetItemAsync(uow, userContext, request.ItemId);
        if (item.IsFailure)
        {
            return item.Error;
        }

        return Result.Success();
    }
}