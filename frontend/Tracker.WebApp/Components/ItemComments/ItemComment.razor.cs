using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Components.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.ItemComments;

public partial class ItemComment
{
    private bool _isEditing = false;
    private TextWithAttachmentsModel model = new();

    [Parameter, EditorRequired]
    public ItemCommentDto Comment { get; set; }
    [Parameter, EditorRequired]
    public int MaxAttachmentSizeBytes { get; set; }
    [Parameter, EditorRequired]
    public EventCallback OnDelete { get; set; }

    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IItemCommentService CommentService { get; set; } = null!;
    [Inject] IAttachmentService AttachmentService { get; set; } = null!;

    private async Task UpdateCommentAsync()
    {
        foreach (var attachment in model.Attachments)
        {
            await using var stream = attachment.OpenReadStream(MaxAttachmentSizeBytes);
            var result = await AttachmentService.UploadAsync(Comment.Id,
                stream, attachment.ContentType, attachment.Name, AttachmentType.Comment);

            if (result.IsSuccess)
            {
                Comment.Attachments.Add(result.Value);
            }
        }
        var request = new UpdateItemCommentRequest
        {
            Content = model.Text
        };
        var commentResult = await CommentService.UpdateAsync(Comment.Id, Comment.ItemId, request);
        if (commentResult.IsSuccess)
        {
            Comment.Content = model.Text;
        }
        model = new();
        _isEditing = false;
    }

    private async Task DeleteCommentAsync()
    {
        var result = await CommentService.DeleteAsync(Comment.Id, Comment.ItemId);
        if (result.IsSuccess)
        {
            await OnDelete.InvokeAsync();
        }
        _isEditing = false;
    }

    private void StartEditing()
    {
        model.Text = Comment.Content;
        model.Attachments = [];
        _isEditing = true;
    }

    private bool CanChange()
    {
        if (AppState.IsUnauthenticated)
        {
            return false;
        }
        return Comment.UploadedBy.Id == AppState.MyId;
    }

    private void CancelEdit()
    {
        model = new();
        _isEditing = false;
    }
}