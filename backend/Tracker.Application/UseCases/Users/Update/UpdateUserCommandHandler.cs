using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.Update;

public class UpdateUserCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();
        var userId = userContext.GetUserId();

        var currentUser = await uow.UserRepository.GetByIdAsync(userId);
        if (currentUser is null)
        {
            return AuthErrors.Unauthenticated;
        }
        var updatedUser = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (updatedUser is null)
        {
            return Error.NotFound("User");
        }

        if (currentUser.Id != request.UserId && currentUser.Role < GlobalRole.Admin)
        {
            return AuthErrors.Forbidden();
        }

        if (updatedUser.Username != request.Username &&
            await uow.UserRepository.UsernameExistsAsync(request.Username))
        {
            return AuthErrors.UsernameExists;
        }

        updatedUser.Username = request.Username;
        updatedUser.FirstName= request.FirstName;
        updatedUser.LastName= request.LastName;

        uow.UserRepository.Update(updatedUser);
        return await uow.SaveChangesAsync(cancellationToken);
    }
}