using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemsList
{
    [Parameter,EditorRequired]
    public string Title { get; set; }
    [Parameter,EditorRequired]
    public List<BoardItemDto> Items { get; set; }
}