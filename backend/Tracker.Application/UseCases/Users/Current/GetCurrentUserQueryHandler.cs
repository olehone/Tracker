using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.Current;

public class GetCurrentUserQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    IAvatarStorageService avatarStorageService)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request, 
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        return user.ToDto(avatarStorageService.GetUrl(user.Id));
    }
}

