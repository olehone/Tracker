using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Lists;

public partial class ListSummary
{
    [Parameter]
    public required BoardListDto List { get; set; }
}