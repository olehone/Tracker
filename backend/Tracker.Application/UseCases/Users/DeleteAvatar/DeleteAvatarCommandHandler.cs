using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.DeleteAvatar;

public sealed class DeleteAvatarCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    IAvatarStorageService avatarStorageService)
    : IRequestHandler<DeleteAvatarCommand, Result>
{
    public async Task<Result> Handle(
        DeleteAvatarCommand request,
        CancellationToken cancellationToken)
    {
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
            return AuthErrors.Forbidden("You cannot delete avatar of another user");
        }

        await avatarStorageService.DeleteAsync(updatedUser.Id, cancellationToken);

        updatedUser.AvatarUpdatedAt = null;
        uow.UserRepository.Update(updatedUser);
        
        return await uow.SaveChangesAsync(cancellationToken);
    }
}
