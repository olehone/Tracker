using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services;

namespace Tracker.WebApp.Components.Attachments;

public partial class FileView
{
    [Parameter, EditorRequired]
    public FileDto File { get; set; }

    [Inject] IItemAttachmentService Attachments { get; set; }
    [Inject] IJSRuntime JS { get; set; }

    private string? _imageUrl;

    protected override async Task OnParametersSetAsync()
    {
        if (IsImage(File))
        {
            await LoadImageUrl();
        }
    }
    private async Task LoadImageUrl()
    {
        var result = await Attachments.DownloadAsync(File.Id, isDirect: false, isRedirect: false);
        if (result.IsSuccess)
        {
            _imageUrl = result.Value;
        }
    }

    private async Task Download()
    {
        var result = await Attachments.DownloadAsync(File.Id, isDirect: false, isRedirect: false);
        if (result.IsSuccess)
        {
            await JS.InvokeVoidAsync("open", result.Value, "_blank");
        }
    }

    private static bool IsImage(FileDto file)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        return imageExtensions.Any(ext =>
            file.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}