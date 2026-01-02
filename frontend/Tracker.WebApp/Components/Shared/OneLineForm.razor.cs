using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Tracker.WebApp.Components.Shared;

public partial class OneLineForm
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public bool AllowEmpty { get; set; } = false;

    [Parameter]
    public Typo Typo { get; set; } = Typo.inherit;

    [Parameter]
    public EventCallback<string> OnValueSaved { get; set; }

    [Parameter]
    public EventCallback<string> OnEditCanceled { get; set; }

    private string? _value;
    private bool _processing;

    protected override void OnParametersSet()
    {
        _value ??= Value;
    }

    private bool IsInputInvalid()
        => !AllowEmpty && string.IsNullOrWhiteSpace(_value);

    private async Task CompleteEdit()
    {
        if (IsInputInvalid())
            return;

        _processing = true;
        await OnValueSaved.InvokeAsync(_value);
        _processing = false;
    }

    private async Task CancelEdit()
    {
        await OnEditCanceled.InvokeAsync();
    }

    private async Task HandleKeys(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await CompleteEdit();
        }
        else if (e.Key == "Escape")
        {
            await CancelEdit();
        }
    }
}