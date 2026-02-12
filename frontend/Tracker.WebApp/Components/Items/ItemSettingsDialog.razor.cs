using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Items;

public partial class ItemSettingsDialog : IDisposable
{
    private const int MaxAttachmentSizeBytes = 50 * 1024 * 1024;

    private bool _openAssign;
    private string _description = string.Empty;
    private DateTime? _date;
    private BoardItemImportance _importance;
    private bool _isEditingDescription = false;
    private bool _openDate = false;
    private List<FileDto> _attachments { get; set; } = null!;


    [Parameter]
    public BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; } = null!;

    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IItemAttachmentService Attachments { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;

    private bool CanChange => IsMeAssigned
        || BoardState.Board.Permissions.CanChangeItem;
    private bool IsMeBoardUser => AppState.IsAuthenticated
        && BoardState.UsersState.IsUserMember(AppState.CurrentUser);
    private bool IsMeAssigned => IsMeBoardUser
        && Item.Assignees.Any(a => a == AppState.MyId);
    private bool IsItemExists =>
        BoardState.ItemsState.BoardItems.Any(i => i.Id == Item.Id);
    private bool Disabled => !CanChange;

    private void ToggleAssign()
    {
        _openAssign = !_openAssign;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
        _description = Item.Description;
        _date = Item.DueDate?.UtcDateTime;
        _importance = Item.Importance;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        var attachments = await Attachments.GetAllAsync(BoardState.Board.Id, Item.Id);
        if (attachments.IsSuccess)
        {
            _attachments = attachments.Value;
            StateHasChanged();
        }
    }

    private void StateHasChangedHandler()
    {
        if (!_isEditingDescription && _description != Item.Description)
        {
            _description = Item.Description;
        }

        _date = Item.DueDate?.UtcDateTime;
        _importance = Item.Importance;
        InvokeAsync(StateHasChanged);
    }

    private async Task UploadAttachmentAsync(IBrowserFile file)
    {
        if (!IsFileValid(file))
        {
            return;
        }
        await using var stream = file.OpenReadStream(MaxAttachmentSizeBytes);
        var result = await Attachments.UploadAsync(BoardState.Board.Id, Item.Id,
            stream, file.ContentType, file.Name);

        if (result.IsSuccess)
        {
            _attachments.Add(result.Value);
        }
    }

    private void DescriptionFocused()
    {
        _isEditingDescription = true;
    }

    private async Task DescriptionBlurred()
    {
        _isEditingDescription = false;

        if (_description == Item.Description)
        {
            return;
        }

        var request = new UpdateBoardItemRequest { Description = _description };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private void StartEditingDescription()
    {
        _isEditingDescription = true;
    }

    private async Task FinishEditingDescription()
    {
        _isEditingDescription = false;

        if (_description == Item.Description)
        {
            return;
        }

        var request = new UpdateBoardItemRequest { Description = _description };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task RemoveDueDate()
    {
        var request = new UpdateBoardItemRequest
        {
            ClearDueDate = true
        };
        _openDate = false;

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task DateSelected(DateTime? date)
    {
        if (date is null)
        {
            return;
        }
        if (_date == date)
        {
            return;
        }
        _date = date;
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(date.Value);
        var dueDate = new DateTimeOffset(
            date.Value.Year,
            date.Value.Month,
            date.Value.Day,
            23,
            59,
            59,
            localOffset);
        var request = new UpdateBoardItemRequest
        {
            DueDate = dueDate
        };

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task ImportanceSelected(BoardItemImportance importance)
    {
        if (_importance == importance)
        {
            return;
        }
        _importance = importance;
        var request = new UpdateBoardItemRequest
        {
            Importance = importance
        };

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private bool IsFileValid(IBrowserFile file)
    {
        if (file is null)
        {
            Snackbar.Add("Attachment is not selected", Severity.Warning);
            return false;
        }

        if (file.Size == 0)
        {
            Snackbar.Add("Attachment is empty", Severity.Warning);
            return false;
        }

        if (file.Size > MaxAttachmentSizeBytes)
        {
            var size = UiHelper.FileSize(MaxAttachmentSizeBytes);
            Snackbar.Add($"Attachment must be less than or equal to {size}", Severity.Warning);
            return false;
        }
        return true;
    }

    public void Dispose()
    {
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
    }
}