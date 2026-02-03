using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.DeleteAvatar;

public class DeleteAvatarCommand : IRequest<Result>
{
    public required Guid UserId { get; set; }
}
