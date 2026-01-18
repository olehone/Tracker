using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}