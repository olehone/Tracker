using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAssignees.Remove;

public class RemoveAssigneeFromItemCommand: IRequest<Result<IReadOnlySet<Guid>>>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required Guid UserId { get; set; }
}
