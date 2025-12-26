using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Create;

public class CreateBoardItemCommand: IRequest<Result<BoardItemDto>>
{
    public required Guid BoardListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
