using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Archive;

public class ArchiveBoardCommand : IRequest<Result>
{
    public required Guid Id { get; set; }
}
