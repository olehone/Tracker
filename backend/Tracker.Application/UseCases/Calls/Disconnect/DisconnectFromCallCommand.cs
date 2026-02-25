using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Disconnect;

public class DisconnectFromCallCommand : IRequest<Result<(Guid UserId, Guid CallId)>>
{
    public required string ConnectionId { get; set; }
}