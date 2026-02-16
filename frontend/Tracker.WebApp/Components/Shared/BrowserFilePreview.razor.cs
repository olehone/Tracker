using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Tracker.WebApp.Components.Shared;

public partial class BrowserFilePreview
{
    [Parameter, EditorRequired]
    public IBrowserFile File { get; set; } = null!;

    [Parameter]
    public EventCallback OnDelete { get; set; }

    private string? _previewDataUrl;
    private bool _isLoadingPreview;

    protected override async Task OnParametersSetAsync()
    {
        var isImage = File.ContentType.StartsWith("image/");
        if (File.Size == 0)
        {
            return;
        }

        if (isImage)
        {
            _isLoadingPreview = true;
            var resizedImage = await File.RequestImageFileAsync(File.ContentType, 150, 150);

            var maxSize = 512000;
            using var stream = resizedImage.OpenReadStream(maxSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            _previewDataUrl = $"data:{File.ContentType};base64,{base64}";
            _isLoadingPreview = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _previewDataUrl = null;
    }
}