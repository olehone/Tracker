using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Attachments;

public partial class FileView
{
    [Parameter, EditorRequired]
    public FileDto File { get; set; }
    [Parameter, EditorRequired]
    public bool Disabled { get; set; }

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] IAttachmentService Attachments { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;

    private string? _imageUrl;

    protected override async Task OnInitializedAsync()
    {
        if (File.IsDeleted)
        {
            return;
        }

        if (UiHelper.IsImage(File))
        {
            await LoadImageUrlAsync();
        }
    }

    private async Task LoadImageUrlAsync()
    {
        var result = await Attachments.GetUrlAsync(File.Id, File.Type);
        if (result.IsSuccess)
        {
            _imageUrl = result.Value;
        }
    }

    private async Task DownloadAsync()
    {
        var result = await Attachments.GetUrlAsync(File.Id, File.Type, isRedirect: false);
        if (result.IsSuccess)
        {
            await JS.InvokeVoidAsync("open", result.Value, "_blank");
        }
    }

    private async Task CopyLinkToClipboardAsync()
    {
        try
        {
            var link = $"api/attachments/{File.Id}";
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", link);
            Snackbar.Add("Copied link to clipboard", Severity.Normal);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot write to clipboard: {ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        var confirmed = await DialogService.ShowMessageBox(
            title: "Warning",
            message: $"You are going to delete {File.FileName}",
            yesText: "Delete",
            cancelText: "Cancel",
            options: new DialogOptions { FullWidth = false });

        if (confirmed != true)
        {
            return;
        }

        var result = await Attachments.DeleteAsync(File.Id, File.Type);
        if (result.IsSuccess)
        {
            File.IsDeleted = true;
        }
    }

    private string GetClass()
    {
        var baseClass = "border-solid rounded mud-width-full pa-0";
        if (File.IsDeleted)
        {
            baseClass += $" mud-theme-{Color.Error.ToDescriptionString()}";
        }
        return baseClass;
    }
}