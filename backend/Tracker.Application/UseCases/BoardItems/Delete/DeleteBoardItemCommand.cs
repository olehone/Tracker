using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Delete;

public class DeleteBoardItemCommand : IRequest<Result>
{
    public required Guid BoardItemId { get; set; }
}