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
    IAvatarStorageService avatarStorageService)
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
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        if (user.Id != request.UserId && user.Role < GlobalRole.Admin)
        {
            return AuthErrors.Forbidden();
        }

        user.AvatarUpdatedAt = DateTimeOffset.UtcNow;
        uow.UserRepository.Update(user);

        var url = await avatarStorageService.UploadAsync(request.Content, request.ContentType, request.UserId, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return url;
    }
}
