using Refit;
using Tracker.Domain.Requests;

namespace Tracker.Services.ApiClients;

internal interface IFaqApi
{
    [Post("/api/faq/ask")]
    Task<IApiResponse<string>> AskAsync([Body] FaqRequest question);
}
