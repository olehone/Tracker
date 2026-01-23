using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommand : IRequest<Result<BoardItemDto>>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public BoardItemImportance? Importance { get; set; }
}