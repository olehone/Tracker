using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.UploadAvatar;

public class UploadAvatarCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    IAvatarStorageService avatars)
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        UploadAvatarCommand request,
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

        updatedUser.AvatarUpdatedAt = DateTimeOffset.UtcNow;
        
        var url = await avatars.UploadAsync(request.Content, request.ContentType, request.UserId, cancellationToken);
        
        uow.UserRepository.Update(updatedUser);
        await uow.SaveChangesAsync(cancellationToken);

        return url;
    }
}
