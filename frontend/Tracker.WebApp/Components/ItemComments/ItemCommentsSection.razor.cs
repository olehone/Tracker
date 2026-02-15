using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
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
    private List<ItemCommentDto> _comments = [];
    private string[] _errors = [];
    private bool _hasMore = true;
    private bool _isLoading = true;
    private DateTimeOffset? _lastLoadedAt = null;
    private ElementReference _trigger;
    private DotNetObjectReference<ItemCommentsSection>? _ref;
    private ItemCommentDto? _selectedComment;
    private MudMenu? _contextMenu;

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
            Amount = 20,
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
        _comments.AddRange(result.Value.Items);
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
        _comments.Insert(0, comment);
        model = new();
    }

    private void EditComment()
    {
        if (_selectedComment is not null)
        {
        }
    }

    private void DeleteComment()
    {
        if (_selectedComment is not null)
        {
        }
    }

    private async Task RightClickComment(MouseEventArgs args, ItemCommentDto comment)
    {
        _selectedComment = comment;
        if (_contextMenu != null)
        {
            await _contextMenu.OpenMenuAsync(args);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
    }
}