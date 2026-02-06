using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetAvatarUrl;

public class GetAvatarUrlCommand : IRequest<Result<string>>
{
    public required Guid UserId { get; set; }
}
