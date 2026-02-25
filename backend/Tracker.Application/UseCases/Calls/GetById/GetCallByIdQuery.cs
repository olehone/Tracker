using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.Get;

public class GetCallByIdQuery : IRequest<Result<CallDto>>
{
    public required Guid Id { get; set; }
}
