using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Leave;

public class LeaveCallCommandHandler(IUnitOfWorkFactory unitOfWorkFactory,
    ICallRepository repo,
    IUserContext userContext)
    : IRequestHandler<LeaveCallCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(LeaveCallCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var userId = userContext.GetUserId();

        var call = await repo.GetCallByIdAsync(request.CallId);
        if (call is null)
        {
            return Error.NotFound("Call");
        }

        var user = call.Users.FirstOrDefault(u => u.User.Id == userId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        call.Users.Remove(user);

        if(call.Users.Count == 0)
        {
            await repo.RemoveCallAsync(call.Id);
        }
        else
        {
            await repo.SaveCallAsync(call);
        }

        return userId;
    }
}
