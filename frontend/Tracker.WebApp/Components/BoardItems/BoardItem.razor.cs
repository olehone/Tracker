using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItem
{

    [Parameter]
    public required BoardItemDto Item { get; set; }

}