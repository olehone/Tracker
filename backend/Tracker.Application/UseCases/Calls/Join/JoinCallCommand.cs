using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Join;

public class JoinCallCommand : IRequest<Result<UserDto>>
{
    public required Guid CallId { get; set; }
    public required string ConnectionId { get; set; }
}
