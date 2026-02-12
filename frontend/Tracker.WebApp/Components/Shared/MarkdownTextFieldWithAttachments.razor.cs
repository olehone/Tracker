using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using PSC.Blazor.Components.MarkdownEditor.EventsArgs;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.Attachments;

namespace Tracker.WebApp.Components.Shared;

public partial class MarkdownTextFieldWithAttachments
{
    private const string _togglePreviewName = "toggle-preview";

    private bool _isPreview = true;

    [Parameter, EditorRequired]
    public string Value { get; set; }
    [Parameter, EditorRequired]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string Header { get; set; }
    [Parameter, EditorRequired]
    public List<FileDto> Attachments { get; set; }
    [Parameter, EditorRequired]
    public EventCallback<IBrowserFile?> UploadAsync { get; set; }
    [Parameter, EditorRequired]
    public bool Disabled { get; set; }

    private RenderFragment RenderMarkdownWithAttachments(string markdown) => builder =>
    {
        if (string.IsNullOrEmpty(markdown))
            return;

        var html = Markdown.ToHtml(markdown);
        var document = Markdown.Parse(markdown);
        var attachmentLinks = document.Descendants<LinkInline>()
            .Where(link => link.Url?.StartsWith("api/attachments/") == true)
            .ToList();

        if (!attachmentLinks.Any())
        {
            builder.AddMarkupContent(0, html);
            return;
        }

        int seq = 0;
        var remainingHtml = html;

        foreach (var link in attachmentLinks)
        {
            var id = link.Url.Split('/').Last();
            if (!Guid.TryParse(id, out var idGuid))
            {
                continue;
            }

            var file = Attachments?.FirstOrDefault(a => a.Id == idGuid);
            if (file == null)
            {
                continue;
            }

            var linkHtml = $"<a href=\"api/attachments/{idGuid}\">";
            var linkEndHtml = "</a>";

            var linkStart = remainingHtml.IndexOf(linkHtml);
            if (linkStart == -1)
            {
                continue;
            }

            var linkEnd = remainingHtml.IndexOf(linkEndHtml, linkStart);
            if (linkEnd == -1)
            {
                continue;
            }

            var beforeLink = remainingHtml.Substring(0, linkStart);
            if (!string.IsNullOrEmpty(beforeLink))
            {
                builder.AddMarkupContent(seq++, beforeLink);
            }

            builder.OpenComponent<FileView>(seq++);
            builder.AddAttribute(seq++, "File", file);
            builder.AddAttribute(seq++, "Disabled", Disabled);
            builder.CloseComponent();

            remainingHtml = remainingHtml.Substring(linkEnd + linkEndHtml.Length);
        }

        if (!string.IsNullOrEmpty(remainingHtml))
        {
            builder.AddMarkupContent(seq++, remainingHtml);
        }
    };

    private async Task UploadAndAddAsync(IBrowserFile file)
    {
        await UploadAsync.InvokeAsync(file);

    }

    private Task OnCustomButtonClicked(MarkdownButtonEventArgs eventArgs)
    {
        if (eventArgs.Name == _togglePreviewName)
        {
            TogglePreview();
        }
        return Task.CompletedTask;
    }

    private void TogglePreview()
    {
        _isPreview = !_isPreview;
    }
}