using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Delete;

public class DeleteBoardCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
}