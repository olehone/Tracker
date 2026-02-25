using Refit;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

internal interface ICallApi
{
    [Get("/api/call/{id}")]
    Task<IApiResponse<CallDto>> GetByIdAsync(Guid id);
}
