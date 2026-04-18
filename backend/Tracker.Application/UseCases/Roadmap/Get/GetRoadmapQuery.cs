using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Roadmap.Get;

public class GetRoadmapQuery : IRequest<Result<RoadmapDto>>
{
    public required Guid BoardId { get; set; }
}
