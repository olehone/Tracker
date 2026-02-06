using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Tracker.Domain.Dtos;
using Tracker.Domain.Options;
using Tracker.Services;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Attachments;

public partial class FileView
{
    [Parameter, EditorRequired]
    public FileDto File { get; set; }
    [Inject] IItemAttachmentService Attachments { get; set; }
    [Inject] IConfiguration Configuration { get; set; }
    [Inject] IAuthService AuthService { get; set; }
    private string _downloadUrl = null!;

    [Inject] IOptions<ApiOptions> apiOptions { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _downloadUrl = await GetDownloadUrl();
    }
    
    private async Task Download()
    {
        await Attachments.DownloadAsync(File.Id, false, false);

    }

    private async Task<string> GetDownloadUrl()
    {
        var token = await AuthService.GetAccessTokenAsync();
        var apiBaseUrl = apiOptions.Value.ApiBaseUrl;
        return $"{apiBaseUrl}/attachments/{File.Id}?access_token={token}";
    }

}