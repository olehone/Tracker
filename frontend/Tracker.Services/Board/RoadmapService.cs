using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Roadmap;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Board;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Board;

public class RoadmapService(IApiErrorHandler apiErrorHandler, IRoadmapApi api) : IRoadmapService
{
    public Task<Result<RoadmapDto>> GetAsync(Guid boardId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAsync(boardId));
    }

    public Task<Result> SaveAsync(Guid boardId, SaveRoadmapRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.SaveAsync(boardId, request));
    }
}