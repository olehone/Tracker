using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Users;

public partial class UserGlobalRoleChip
{
    [Parameter]
    public GlobalRole Role { get; set; }
}