using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tracker.WebApp.Components.Shared;

public partial class Date
{
    [Parameter, EditorRequired]
    public DateTimeOffset Value { get; set; }
    [Parameter]
    public bool IsDone { get; set; }

    private string FormattedDate()
    {
        if (Value.Year != DateTimeOffset.Now.Year)
        {
            return Value.ToString("MMM d, yyyy");
        }
        return Value.ToString("MMM d");
    }

    private Color DateColor()
    {
        if (IsDone)
        {
            return Color.Success;
        }
        if (Value < DateTime.Now)
        {
            return Color.Error;
        }
        if (Value < DateTimeOffset.Now.AddDays(1))
        {
            return Color.Warning;
        }

        return Color.Default;
    }
}