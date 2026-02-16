using System.ComponentModel.Design;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Components.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.ItemComments;

public partial class ItemCommentsSection : IAsyncDisposable
{
    private TextWithAttachmentsModel model = new();
    private List<List<ItemCommentDto>> _oldCommentGroups = [];
    private List<List<ItemCommentDto>> _newCommentGroups = [];
    private string[] _errors = [];
    private bool _hasMore = true;
    private bool _isLoading = true;
    private DateTimeOffset? _lastLoadedAt = null;
    private ElementReference _trigger;
    private DotNetObjectReference<ItemCommentsSection>? _ref;

    [Parameter, EditorRequired]
    public Guid ItemId { get; set; }
    [Parameter, EditorRequired]
    public Func<IBrowserFile, bool> IsFileValid { get; set; }
    [Parameter, EditorRequired]
    public int MaxAttachmentSizeBytes { get; set; }
    [Parameter]
    public bool Disabled { get; set; } = true;

    [Inject] IItemCommentService CommentService { get; set; } = null!;
    [Inject] IAttachmentService AttachmentService { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    private bool IsMine(Guid id)
    {
        if (AppState.IsUnauthenticated)
        {
            return false;
        }
        return id == AppState.MyId;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _ref = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("observeElement", _trigger, _ref);
        }
    }

    [JSInvokable]
    public async Task ScrolledAsync()
    {
        await LoadCommentsAsync();
    }

    private async Task LoadCommentsAsync()
    {
        if (!_hasMore)
        {
            return;
        }
        _isLoading = true;
        var request = new CursorTimeRequest
        {
            Amount = 5,
            Before = _lastLoadedAt
        };
        var result = await CommentService.GetAsync(ItemId, request);
        _isLoading = false;
        if (result.IsFailure)
        {
            _hasMore = false;
            return;
        }
        _hasMore = result.Value.HasMore;
        _lastLoadedAt = result.Value.LastLoadedAt;
        AddComments(result.Value.Items);
        StateHasChanged();
    }

    private void AddComments(IReadOnlyList<ItemCommentDto> comments)
    {
        Guid? lastUserId = _oldCommentGroups.Count == 0
            ? null
            : _oldCommentGroups.Last().Last().UploadedBy.Id;
        foreach (var comment in comments)
        {
            if (lastUserId is null
                || _oldCommentGroups.Count == 0)
            {
                _oldCommentGroups.Add([comment]);
                lastUserId = comment.UploadedBy.Id;
                continue;
            }

            if (comment.UploadedBy.Id == lastUserId)
            {
                _oldCommentGroups.Last().Add(comment);
            }
            else
            {
                _oldCommentGroups.Add([comment]);
            }

            lastUserId = comment.UploadedBy.Id;
        }
    }

    public void ApplyCommentCreated(ItemCommentDto comment)
    {
        if (_newCommentGroups.Count > 1)
        {
            if (comment.UploadedBy.Id == _newCommentGroups.First().First().UploadedBy.Id)
            {
                _newCommentGroups.First().Insert(0, comment);
            }
            else
            {
                _newCommentGroups.Insert(0, [comment]);
            }
        }
        else
        {
            if (comment.UploadedBy.Id == _oldCommentGroups.First().First().UploadedBy.Id)
            {
                _oldCommentGroups.First().Insert(0, comment);
            }
            else
            {
                _newCommentGroups.Insert(0, [comment]);
            }
        }
        StateHasChanged();
    }

    public void ApplyCommentUpdated(ItemCommentDto updatedComment)
    {
        var oldComment = _oldCommentGroups.SelectMany(g => g).FirstOrDefault(c => c.Id == updatedComment.Id);
        if (oldComment is not null)
        {
            //_oldCommentGroups = _oldCommentGroups
            //    .Select(g => g
            //        .Select(c => c.Id == updatedComment.Id ? updatedComment : c)
            //        .ToList())
            //    .ToList();
            oldComment = updatedComment;
            StateHasChanged();
            return;
        }

        var newComment = _newCommentGroups.SelectMany(g => g).FirstOrDefault(c => c.Id == updatedComment.Id);
        if (newComment is not null)
        {
            //_oldCommentGroups = _oldCommentGroups
            //    .Select(g => g
            //        .Select(c => c.Id == updatedComment.Id ? updatedComment : c)
            //        .ToList())
            //    .ToList();
            newComment = updatedComment;
            StateHasChanged();
            return;
        }
    }

    public void DeleteCommentInGroup(List<List<ItemCommentDto>> groups, List<ItemCommentDto> group, Guid commentId)
    {
        var comment = group.First(c => c.Id == commentId);
        group.Remove(comment);
        if (group.Count == 0)
        {
            groups.Remove(group);
        }
        StateHasChanged();
    }

    public void ApplyCommentDeleted(Guid commentId)
    {
        var group = _oldCommentGroups
            .FirstOrDefault(g => g.Any(c => c.Id == commentId));
        if (group is not null)
        {
            DeleteCommentInGroup(_oldCommentGroups, group, commentId);
            return;
        }
        group = _newCommentGroups
            .FirstOrDefault(g => g.Any(c => c.Id == commentId));
        if (group is not null)
        {
            DeleteCommentInGroup(_newCommentGroups, group, commentId);
            return;
        }
    }

    private async Task CreateCommentAsync()
    {
        var request = new CreateCommentRequest { Content = model.Text };
        var createCommentResult = await CommentService.CreateAsync(ItemId, request);
        if (createCommentResult.IsFailure)
        {
            if (createCommentResult.Error.Type == ErrorType.Validation)
            {
                _errors = createCommentResult.Error.Details
                    ?? [createCommentResult.Error.Description];
                StateHasChanged();
            }
            return;
        }
        var comment = createCommentResult.Value;

        foreach (var attachment in model.Attachments)
        {
            await using var stream = attachment.OpenReadStream(MaxAttachmentSizeBytes);
            var result = await AttachmentService.UploadAsync(comment.Id,
                stream, attachment.ContentType, attachment.Name, AttachmentType.Comment);

            if (result.IsSuccess)
            {
                comment.Attachments.Add(result.Value);
            }
        }

        ApplyCommentCreated(comment);
        model = new();
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
    }
}