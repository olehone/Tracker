using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardLists;
public partial class BoardList
{

    [Parameter]
    public required BoardListDto List { get; set; }
    [Parameter]
    public EventCallback<string> OnCreateItem { get; set; }

    private async Task CreateNewItem(string title)
    {
        await OnCreateItem.InvokeAsync(title);
    }

}