using Microsoft.AspNetCore.Components.Forms;

namespace Tracker.WebApp.Components.Shared;

public class TextWithAttachmentsModel
{
    public string Text { get; set; }
    public IReadOnlyList<IBrowserFile> Attachments { get; set; }
}
