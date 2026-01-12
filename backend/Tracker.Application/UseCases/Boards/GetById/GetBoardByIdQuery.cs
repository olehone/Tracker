using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetById;

public class GetBoardByIdQuery : IRequest<Result<BoardFullDto>>
{
    public required Guid Id { get; set; }
}
