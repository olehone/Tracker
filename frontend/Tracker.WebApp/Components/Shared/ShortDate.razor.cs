using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tracker.WebApp.Components.Shared;

public partial class ShortDate
{
    [Parameter, EditorRequired]
    public DateTimeOffset Value { get; set; }
    [Parameter]
    public bool IsDone { get; set; } = false;
    [Parameter]
    public bool HasAttention { get; set; } = false;

    public static string GetFormattedDate(DateTimeOffset date)
    {
        if (date.Year != DateTimeOffset.Now.Year)
        {
            return date.ToString("MMM d, yyyy");
        }
        return date.ToString("MMM d");
    }

    public static string GetFormattedDate(DateTime? date)
    {
        if (!date.HasValue)
        {
            return "Undefined";
        }
        if (date.Value.Year != DateTimeOffset.Now.Year)
        {
            return date.Value.ToString("MMM d, yyyy");
        }
        return date.Value.ToString("MMM d");
    }

    public static Color GetDateColor(DateTimeOffset date, bool isDone)
    {
        if (isDone)
        {
            return Color.Success;
        }
        if (date < DateTime.Now)
        {
            return Color.Error;
        }
        if (date < DateTimeOffset.Now.AddDays(1))
        {
            return Color.Warning;
        }

        return Color.Default;
    }

    public static Color GetDateColor(DateTime? date, bool isDone)
    {
        if (isDone)
        {
            return Color.Success;
        }
        if (date < DateTime.Now)
        {
            return Color.Error;
        }
        if (date < DateTimeOffset.Now.AddDays(1))
        {
            return Color.Warning;
        }

        return Color.Default;
    }

    private Variant GetVariant()
    {
        return HasAttention
            ? Variant.Filled
            : Variant.Outlined;
    }

    private string GetFromattedDate()
    {
        return GetFormattedDate(Value);
    }

    private Color GetDateColor()
    {
        return GetDateColor(Value, IsDone);
    }
}