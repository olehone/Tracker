using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.ItemComments;

public partial class ItemCommentsSection : IAsyncDisposable
{
    private List<ItemCommentDto> _comments = [];
    private string[] _errors = [];
    private bool _hasMore = true;
    private bool _isLoading = true;
    private DateTimeOffset? _lastLoadedAt = null;
    private ICollection<IBrowserFile> _attachments = [];
    private List<FileDto> _newAttachments = [];
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

    public string NewComment { get; set; } = string.Empty;

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

    private async Task CreateComment(string content)
    {
        var request = new CreateCommentRequest { Content = content };
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

        foreach (var attachment in _attachments)
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
    }

    private async Task AddAttachmentsAsync(List<IBrowserFile> files)
    {
        foreach (var file in files)
        {
            if (!IsFileValid(file))
            {
                return;
            }
            _attachments.Add(file);
            StateHasChanged();
        }
    }

    private async Task AddAttachmentAsync(IBrowserFile file)
    {
        if (!IsFileValid(file))
        {
            return;
        }
        _attachments.Add(file);
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
    }
}