using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.ItemComments;

public partial class ItemCommentsSection : IAsyncDisposable
{
    private List<ItemCommentDto> _comments = [];
    private bool _hasMore = true;
    private bool _isLoading = true;
    private DateTimeOffset? _lastLoadedAt = null;

    private ElementReference _trigger;
    private DotNetObjectReference<ItemCommentsSection>? _ref;

    [Parameter, EditorRequired]
    public Guid ItemId { get; set; }

    [Inject] IItemCommentService CommentService { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        _comments.Clear();
        _lastLoadedAt = null;
        _hasMore = true;
        await LoadCommentsAsync();
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
        _comments.AddRange(result.Value.Items);
        StateHasChanged();
    }

    private async Task CreateComment(string content)
    {
        var request = new CreateCommentRequest { Content = content };
        var result = await CommentService.CreateAsync(ItemId, request);
        if (result.IsSuccess)
        {
            _comments.Insert(0, result.Value);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
    }
}