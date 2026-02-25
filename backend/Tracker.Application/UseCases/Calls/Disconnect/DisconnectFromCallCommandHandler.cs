using MediatR;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Disconnect;

public class DisconnectFromCallCommandHandler(IUnitOfWorkFactory unitOfWorkFactory,
    ICallRepository repo)
    : IRequestHandler<DisconnectFromCallCommand, Result<DisconnectInfo?>>
{
    public async Task<Result<DisconnectInfo?>> Handle(DisconnectFromCallCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var call = await repo.GetCallByConnectionAsync(request.ConnectionId);
        if (call is null)
        {
            return Result.SuccessOf<DisconnectInfo?>(null);
        }

        var user = call.Users.FirstOrDefault(u => u.ConnectionId == request.ConnectionId);
        if (user is null)
        {
            return Result.SuccessOf<DisconnectInfo?>(null);
        }

        call.Users.Remove(user);

        LeaveInfo leaveInfo;
        if (call.Users.Any(u => u.Status == CallUserStatus.Joined))
        {
            await repo.SaveCallAsync(call);
            leaveInfo = new LeaveInfo(user.User.Id, false, []);
        }
        else
        {
            await repo.RemoveCallAsync(call.Id);
            var connectIds = call.Users.Select(u => u.ConnectionId).ToList();
            leaveInfo = new LeaveInfo(call.Id, true, connectIds);
        }

        return new DisconnectInfo(user.User.Id, leaveInfo);
    }
}
