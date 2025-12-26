using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Create;

public class CreateBoardCommand : IRequest<Result<BoardSummaryDto>>
{
    public Guid WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}
