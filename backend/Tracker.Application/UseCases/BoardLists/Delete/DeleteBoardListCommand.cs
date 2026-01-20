using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Delete;

public class DeleteBoardListCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardListId { get; set; }
}