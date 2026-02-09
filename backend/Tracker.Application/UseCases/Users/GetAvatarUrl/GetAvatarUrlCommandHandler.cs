using MediatR;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetAvatarUrl;

public sealed class GetAvatarUrlCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAvatarStorageService avatarStorageService)
    : IRequestHandler<GetAvatarUrlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        GetAvatarUrlCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        return await avatarStorageService.GetUrlAsync(user.Id, cancellationToken);
    }
}
