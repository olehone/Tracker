using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages.Admin;

public partial class Workspaces
{
    
    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;

}