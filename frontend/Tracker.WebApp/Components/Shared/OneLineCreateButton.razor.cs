using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tracker.WebApp.Components.Shared;

public partial class OneLineCreateButton
{
    private bool _isAddingItem;
    [Parameter]
    public string Title { get; set; } = "Add";
    [Parameter]
    public Size Size { get; set; } = Size.Large;
    [Parameter]
    public Typo Typo { get; set; } = Typo.inherit;
    [Parameter]
    public bool IsWithFrame { get; set; } = true;
    [Parameter, EditorRequired]
    public EventCallback<string> OnCreate { get; set; }

    private void AddItemPressed()
    {
        _isAddingItem = true;
    }

    private async Task SubmitNewItem(string value)
    {
        await OnCreate.InvokeAsync(value);
        _isAddingItem = false;
    }

    private void ClosePressed()
    {
        _isAddingItem = false;
    }
}