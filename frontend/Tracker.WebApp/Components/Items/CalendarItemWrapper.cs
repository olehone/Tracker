using Heron.MudCalendar;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Items;

public class CalendarItemWrapper : CalendarItem
{
    public BoardItemDto Item { get; set; } = null!;

    public CalendarItemWrapper() { }

    public CalendarItemWrapper(BoardItemDto item)
    {
        if (!item.DueDate.HasValue)
        {
            throw new ArgumentException("Cannot add item without due date to calendar");
        }

        Item = item;
        Text = item.Title;
        AllDay = true;
        Start = item.DueDate.Value.Date;
    }
}