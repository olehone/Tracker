using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Items;

public partial class ItemsQuadrant
{
    [Parameter, EditorRequired]
    public string Title { get; set; }
    [Parameter, EditorRequired]
    public List<BoardItemDto> Items { get; set; }
    [Parameter]
    public string Color { get; set; } = string.Empty;
}