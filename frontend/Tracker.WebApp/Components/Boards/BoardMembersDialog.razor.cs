using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardMembersDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

}