using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Roadmap.Get;

public class GetRoadmapQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetRoadmapQuery, Result<RoadmapDto>>
{
    public async Task<Result<RoadmapDto>> Handle(GetRoadmapQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        return await uow.RoadmapRepository
            .GetByBoardIdAsync(request.BoardId, cancellationToken);
    }
}
