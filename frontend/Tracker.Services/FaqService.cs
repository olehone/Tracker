using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

internal class FaqService(IApiErrorHandler apiErrorHandler,
    IFaqApi api) : IFaqService
{
    public Task<Result<string>> AskAsync(string question)
    {
        var request = new FaqRequest
        {
            Question = question
        };
        return apiErrorHandler.ExecuteAsync(() => api.AskAsync(request));
    }
}
