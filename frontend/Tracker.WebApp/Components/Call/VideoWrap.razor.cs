using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tracker.WebApp.Components.Call;

public partial class VideoWrap
{
    [Parameter, EditorRequired]
    public string Id { get; set; }
    [Parameter, EditorRequired]
    public bool IsOwn { get; set; }
    [Parameter, EditorRequired]
    public bool IsMuted { get; set; }
    [Parameter, EditorRequired]
    public bool IsCameraOff { get; set; }
    [Parameter]
    public EventCallback<string>? OnClick { get; set; }


    private async Task Click()
    {
        if (!OnClick.HasValue)
        {
            return;
        }
        await OnClick.Value.InvokeAsync(Id);
    }

    private string GetStyle()
    {
        var style = string.Empty;
        if (IsCameraOff)
        {
            style += $"background-color: {Colors.Gray.Darken4.ToString()};";
        }
        return style;
    }
}