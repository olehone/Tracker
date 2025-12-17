using System.Net.Http.Headers;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthService _auth;

    public AuthHeaderHandler(IAuthService auth)
        => _auth = auth;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _auth.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
