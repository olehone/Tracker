using Microsoft.AspNetCore.Components.Forms;

namespace Tracker.WebApp.Components.Shared;

public class TextWithAttachmentsModel
{
    public string Text { get; set; } = string.Empty;
    public IReadOnlyList<IBrowserFile> Attachments { get; set; } = new List<IBrowserFile>();
}
