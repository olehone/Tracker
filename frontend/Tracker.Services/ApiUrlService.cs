using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

internal class ApiUrlService (IOptions<ApiOptions> options) : IApiUrlService
{
    public string GetApiUrl()
    {
        return options.Value.ApiBaseUrl;
    }
}
