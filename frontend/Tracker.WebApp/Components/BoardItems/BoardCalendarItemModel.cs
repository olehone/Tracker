using Heron.MudCalendar;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardItems;

public class BoardCalendarItemModel : CalendarItem
{
    public BoardItemDto BoardItem { get; set; }

    public BoardCalendarItemModel() : base()
    {
    }

    public BoardCalendarItemModel(BoardItemDto item)
    {
        if (!item.DueDate.HasValue)
        {
            throw new ArgumentException("Cannot convert item without due date to calendar model");
        }
        BoardItem = item;

        Text = item.Title;
        Start = item.DueDate.Value.DateTime.AddHours(-1);
        AllDay = true;

    }
}