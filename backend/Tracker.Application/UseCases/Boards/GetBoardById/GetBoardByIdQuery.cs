using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetBoardById;

public class GetBoardByIdQuery : IRequest<Result<BoardFullDto>>
{
    public Guid Id { get; set; }
}
