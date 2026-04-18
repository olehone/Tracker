using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Roadmap;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Board;

public interface IRoadmapService
{
    Task<Result<RoadmapDto>> GetAsync(Guid boardId);
    Task<Result> SaveAsync(Guid boardId, SaveRoadmapRequest request);

}