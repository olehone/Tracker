using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.UserSubscriptions;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetCurrentPermissions;

public class GetCurrentUserPermissionsQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext)
    : IRequestHandler<GetCurrentUserPermissionsQuery, Result<UserPermissionsDto>>
{
    public async Task<Result<UserPermissionsDto>> Handle(
        GetCurrentUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return SubscriptionPolicy.None;
        }

        var userId = userContext.GetUserId();
        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var subscription = await uow.UserSubscriptionRepository.GetByUserIdAsync(userId);

        var permissions = SubscriptionPolicy.GetPermissions(user.Role, subscription?.Plan);
        return permissions;
    }
}