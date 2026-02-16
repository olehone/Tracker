using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Shared;

public partial class FormWithAttachments
{
    private MudFileUpload<IReadOnlyList<IBrowserFile>> _mudFileUpload;
    private MudForm form;
    private TextWithAttachmentsModelValidator ValidationRules;
    private bool _isSubmitting;
    private bool IsEditing => !string.IsNullOrWhiteSpace(Model.Text);

    [Parameter]
    public int MaxTextLength { get; set; } = 500;
    [Parameter]
    public long MaxFileSizeBytes { get; set; } = 104857600;
    [Parameter]
    public int MaxFileCount { get; set; } = 5;
    [Parameter]
    public string AcceptedExtensions { get; set; } = "*";

    [Parameter]
    public Func<int> ExistingAttachmentsCount { get; set; } = (() => 0);
    [Parameter]
    public EventCallback OnClear { get; set; }
    [Parameter, EditorRequired]
    public EventCallback<TextWithAttachmentsModel> OnSubmit { get; set; }
    [Parameter, EditorRequired]
    public TextWithAttachmentsModel Model { get; set; } = new();
    [Parameter, EditorRequired]
    public EventCallback<TextWithAttachmentsModel> ModelChanged { get; set; }

    protected override void OnInitialized()
    {
        ValidationRules = new TextWithAttachmentsModelValidator(MaxTextLength, MaxFileSizeBytes, MaxFileCount, ExistingAttachmentsCount);
        Model.Attachments = [];
    }

    private async Task Submit()
    {
        await form.Validate();

        if (!form.IsValid)
        {
            return;
        }

        _isSubmitting = true;
        StateHasChanged();

        await OnSubmit.InvokeAsync(Model);
        _isSubmitting = false;
        StateHasChanged();
    }

    private void RemoveFile(IBrowserFile file)
    {
        Model.Attachments = Model.Attachments.Where(f => !f.Equals(file)).ToList();
    }

    private async Task TriggerFileUpload()
    {
        await _mudFileUpload.OpenFilePickerAsync();
    }

    private async Task Clear()
    {
        if (OnClear.HasDelegate)
        {
            await OnClear.InvokeAsync();
        }
        else
        {
            Model = new();
        }
    }

    private async Task HandleKeys(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && e.CtrlKey)
        {
            await Submit();
        }
    }

    public class TextWithAttachmentsModelValidator : AbstractValidator<TextWithAttachmentsModel>
    {
        public TextWithAttachmentsModelValidator(int maxTextLength, long maxFileSizeBytes, int maxFileCount, Func<int> existingFiles)
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text is required")
                .MaximumLength(maxTextLength)
                .WithMessage($"Text cannot exceed {maxTextLength} characters");

            RuleFor(x => x.Attachments)
                .Must(files => files == null || (files.Count + existingFiles()) <= maxFileCount)
                .WithMessage($"Maximum {maxFileCount} files allowed");

            When(x => x.Attachments != null && x.Attachments.Any(), () =>
            {
                RuleFor(x => x.Attachments)
                    .Must(files => files.All(f => f.Size <= maxFileSizeBytes))
                    .WithMessage($"Each file must be less than {UiHelper.FileSize(maxFileSizeBytes)}");
            });
        }
    }
}