using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Leave;

public class LeaveCallCommand : IRequest<Result<LeaveInfo>>
{
    public required Guid CallId { get; set; }
}