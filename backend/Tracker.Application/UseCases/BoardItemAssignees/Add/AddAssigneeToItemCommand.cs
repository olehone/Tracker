using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAssignees.Add;

public class AddAssigneeToItemCommand: IRequest<Result<BoardItemDto>>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required Guid UserId { get; set; }
}
