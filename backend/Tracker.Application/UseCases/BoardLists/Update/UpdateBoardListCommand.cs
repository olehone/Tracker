using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Update;

public class UpdateBoardListCommand : IRequest<Result<BoardListDto>>
{
    public required Guid BoardId { get; set; }
    public Guid BoardListId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}