using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Move;

public class MoveBoardListCommand: IRequest<Result>
{
    public required Guid BoardListId { get; set; }
    public int Position { get; set; }
}
