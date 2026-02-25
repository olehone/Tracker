using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Disconnect;

public class DisconnectFromCallCommandHandler(IUnitOfWorkFactory unitOfWorkFactory,
    ICallRepository repo)
    : IRequestHandler<DisconnectFromCallCommand, Result<(Guid UserId, Guid CallId)>>
{
    public async Task<Result<(Guid UserId, Guid CallId)>> Handle(DisconnectFromCallCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var call = await repo.GetCallByConnectionAsync(request.ConnectionId);
        if (call is null)
        {
            return Error.NotFound("Call", "connection");
        }

        var user = call.Users.FirstOrDefault(u => u.ConnectionId == request.ConnectionId);
        if (user is null)
        {
            return Error.NotFound("User", "connection");
        }

        call.Users.Remove(user);

        if (call.Users.Count == 0)
        {
            await repo.RemoveCallAsync(call.Id);
        }
        else
        {
            await repo.SaveCallAsync(call);
        }

        return (user.User.Id, call.Id);
    }
}
