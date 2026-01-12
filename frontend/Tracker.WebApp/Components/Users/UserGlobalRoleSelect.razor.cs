using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Users;

public partial class UserGlobalRoleSelect
{
    [Parameter]
    public GlobalRole Value { get; set; }
    [Parameter]
    public EventCallback<GlobalRole> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
}