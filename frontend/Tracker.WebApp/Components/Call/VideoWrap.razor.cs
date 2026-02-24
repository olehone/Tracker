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
    public EventCallback<string> OnClick { get; set; }
    [Parameter] public bool Fill { get; set; }

    private string GetStyle()
    {
        var style = "justify-content:center; align-items:center; display:flex; flex-shrink:0; ";
        style += IsCameraOff ? $"background-color: {Colors.Gray.Darken4}; " : "";
        style += Fill
            ? "flex:1; min-height:0; width:100%;"
            : "height:100%; width:auto; aspect-ratio:16/9; min-width:0;";
        return style;
    }

    private string GetVideoStyle()
    {
        return IsCameraOff
            ? "position:absolute; height:0; width:0; visibility:hidden;"
            : "object-fit:contain; height:100%; width:100%;";
    }

    private async Task Click()
    {
        if (!OnClick.HasDelegate)
        {
            return;
        }
        await OnClick.InvokeAsync(Id);
    }
}