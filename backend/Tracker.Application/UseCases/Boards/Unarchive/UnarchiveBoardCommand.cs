using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Unarchive;

public class UnarchiveBoardCommand : IRequest<Result>
{
    public required Guid Id { get; set; }
}
