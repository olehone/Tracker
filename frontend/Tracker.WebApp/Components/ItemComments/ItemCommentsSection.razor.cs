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
    private List<CommentGroup> _commentGroups = [];
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
        Guid? lastUserId = _commentGroups.Count == 0
            ? null
            : _commentGroups.Last().UserId;
        foreach (var comment in comments)
        {
            if (lastUserId is null || _commentGroups.Count == 0)
            {
                _commentGroups.Add(new(comment));
                lastUserId = comment.UploadedBy.Id;
                continue;
            }

            if (comment.UploadedBy.Id == lastUserId)
            {
                _commentGroups.Last().Comments.Add(comment);
            }
            else
            {
                _commentGroups.Add(new(comment));
            }

            lastUserId = comment.UploadedBy.Id;
        }
    }

    public void ApplyCommentCreated(ItemCommentDto comment)
    {
        var firstGroup = _commentGroups.First();
        if (_commentGroups.Count == 0
            || comment.UploadedBy.Id == firstGroup.UserId)
        {
            firstGroup.Comments.Insert(0, comment);
        }
        else
        {
            _commentGroups.Insert(0, new(comment));
        }

        StateHasChanged();
    }

    public void ApplyCommentUpdated(ItemCommentDto updatedComment)
    {
        var comment = _commentGroups.SelectMany(g => g.Comments).FirstOrDefault(c => c.Id == updatedComment.Id);
        if (comment is null)
        {
            return;
        }
        comment = updatedComment;
        StateHasChanged();
    }

    public void ApplyCommentDeleted(Guid commentId)
    {
        var group = _commentGroups
            .FirstOrDefault(g => g.Comments.Any(c => c.Id == commentId));

        if (group is null)
        {
            return;
        }

        var comment = group.Comments.First(c => c.Id == commentId);
        group.Comments.Remove(comment);

        if (group.Comments.Count == 0)
        {
            _commentGroups.Remove(group);
        }
        StateHasChanged();
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
        StateHasChanged();


        ApplyCommentCreated(comment);
        model = new();
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
    }
}