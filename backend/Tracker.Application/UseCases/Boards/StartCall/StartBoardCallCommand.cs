using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.StartCall;

public class StartBoardCallCommand : IRequest<Result<Guid>>
{
    public Guid BoardId { get; set; }
}