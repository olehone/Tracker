using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardLists;

public partial class BoardListSummary
{
    [Parameter]
    public required BoardListDto List { get; set; }
}