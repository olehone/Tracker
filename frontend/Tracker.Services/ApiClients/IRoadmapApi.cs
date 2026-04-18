using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Roadmap;

namespace Tracker.Services.ApiClients;

public interface IRoadmapApi
{
    [Get("/api/boards/{boardId}/roadmap")]
    Task<IApiResponse<RoadmapDto>> GetAsync(Guid boardId);
    [Put("/api/boards/{boardId}/roadmap")]
    Task<IApiResponse> SaveAsync(Guid boardId, [Body] SaveRoadmapRequest request);
}
