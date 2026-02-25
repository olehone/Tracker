using Tracker.Domain.Dtos;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

internal class CallService(IApiErrorHandler apiErrorHandler,
    ICallApi api)
    : ICallService
{
    public Task<Result<CallDto>> GetByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetByIdAsync(id));
    }
}
