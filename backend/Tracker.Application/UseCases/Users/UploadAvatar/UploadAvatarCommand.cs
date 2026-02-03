using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.UploadAvatar;

public class UploadAvatarCommand : IRequest<Result<string>>
{
    public required Guid UserId { get; set; }
    public required Stream File { get; set; }
}
