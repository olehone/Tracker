using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Create;

public class CreateBoardListCommand : IRequest<Result<BoardListDto>>
{
    public required Guid BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
