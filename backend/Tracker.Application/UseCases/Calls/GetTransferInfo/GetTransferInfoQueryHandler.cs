using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.States;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.GetTransferInfo;

public class GetTransferInfoQueryHandler(IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    ICallState repo)
    : IRequestHandler<GetTransferInfoQuery, Result<TransferInfo>>
{
    public async Task<Result<TransferInfo>> Handle(GetTransferInfoQuery request, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(request.TargetUserId, out var targetUserId))
        {
            return Error.Validation([$"Target user id({request.TargetUserId}) is not guid"]);
        }

        await using var uow = unitOfWorkFactory.Create();

        var senderId = userContext.GetUserId();
        var sender = await uow.UserRepository.GetByIdAsync(senderId);

        var call = await repo.GetCallByIdAsync(request.CallId);
        if (call is null)
        {
            return Error.NotFound("Call");
        }

        var target = call.Users.FirstOrDefault(u => u.User.Id == targetUserId);
        if (target is null)
        {
            return Error.NotFound("User");
        }

        return new TransferInfo(senderId.ToString(), target.ConnectionId);
    }
}
