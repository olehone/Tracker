using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Move;

public class MoveBoardItemCommand: IRequest<Result>
{
    public required Guid ToBoardListId { get; set; }
    public required Guid BoardItemId { get; set; }
    public int Position { get; set; }
}
