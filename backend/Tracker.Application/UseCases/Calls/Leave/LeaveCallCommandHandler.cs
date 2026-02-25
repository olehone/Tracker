using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Leave;

public class LeaveCallCommandHandler(IUnitOfWorkFactory unitOfWorkFactory,
    ICallRepository repo,
    IUserContext userContext)
    : IRequestHandler<LeaveCallCommand, Result<LeaveInfo?>>
{
    public async Task<Result<LeaveInfo?>> Handle(LeaveCallCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var userId = userContext.GetUserId();

        var call = await repo.GetCallByIdAsync(request.CallId);
        if (call is null)
        {
            return Result.SuccessOf<LeaveInfo?>(null);
        }

        var user = call.Users.FirstOrDefault(u => u.User.Id == userId);
        if (user is null)
        {
            return Result.SuccessOf<LeaveInfo?>(null);
        }

        call.Users.Remove(user);

        if (call.Users.Any(user => user.Status == CallUserStatus.Joined))
        {
            await repo.SaveCallAsync(call);
            return new LeaveInfo(userId, false, []);
        }

        await repo.RemoveCallAsync(call.Id);
        var connectIds = call.Users.Select(u => u.ConnectionId).ToList();
        return new LeaveInfo(call.Id, true, connectIds);
    }
}
