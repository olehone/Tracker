using System.Net.Http.Headers;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthStorage _authStorage;

    public AuthHeaderHandler(IAuthStorage authStorage)
        => _authStorage = authStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokensDto = await _authStorage.GetAsync();
        var token = tokensDto?.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
