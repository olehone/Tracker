using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Join;

public class JoinCallCommandHandler(IUnitOfWorkFactory unitOfWorkFactory,
    ICallRepository repo,
    IUserContext userContext)
    : IRequestHandler<JoinCallCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(JoinCallCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        var call = await repo.GetCallByIdAsync(request.CallId);
        if (call is null)
        {
            return Error.NotFound("Call");
        }

        var existing = call.Users.FirstOrDefault(u => u.User.Id == user.Id);
        if (existing is not null)
        {
            existing.ConnectionId = request.ConnectionId;
            existing.Status = CallUserStatus.Joined;
        }
        else
        {
            call.Users.Add(new CallUser
            {
                User = user.ToDto(),
                ConnectionId = request.ConnectionId,
                Status = CallUserStatus.Joined
            });
        }

        await repo.SaveCallAsync(call);
        return user.ToDto();
    }
}
