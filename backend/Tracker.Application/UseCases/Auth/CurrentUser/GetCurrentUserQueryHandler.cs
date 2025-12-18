using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.CurrentUser;

public class GetCurrentUserQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request, 
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return Result.FailureOf<UserDto>(AuthErrors.CurrentUserIsNotAuthenticated);
        }

        Guid userId = userContext.GetUserId();
        await using var uow = unitOfWorkFactory.Create();

        User? user = await uow.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result.FailureOf<UserDto>(AuthErrors.UserNotFound);
        }

        return user.ToDto();
    }
}

