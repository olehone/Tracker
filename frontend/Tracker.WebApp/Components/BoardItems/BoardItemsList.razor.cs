using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemsList
{
    private BoardListDto? SelectedList { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        SelectedList = Lists[0];
    }
    private async Task CreateNewItem(string title)
    {
        if (SelectedList is null)
        {
            return;
        }
        await BoardState.ItemsState.CreateAsync(SelectedList.Id, title);
    }
}