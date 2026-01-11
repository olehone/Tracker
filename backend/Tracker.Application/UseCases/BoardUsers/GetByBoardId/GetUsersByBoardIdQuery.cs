using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Get;

public class GetUsersByBoardIdQuery : IRequest<Result<List<BoardUserDto>>>
{
    public required Guid BoardId { get; set; }
}