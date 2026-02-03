using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor;

namespace Tracker.WebApp.Components.Shared;

public partial class CustomImage
{
    private readonly ParameterState<string?> _srcState;

    public CustomImage()
    {
        using var registerScope = CreateRegisterScope();
        _srcState = registerScope.RegisterParameter<string?>(nameof(Src))
            .WithParameter(() => Src);
    }

    protected string Classname =>
        new CssBuilder("mud-image")
            .AddClass("fluid", Fluid)
            .AddClass($"object-{ObjectFit.ToDescriptionString()}")
            .AddClass($"object-{ObjectPosition.ToDescriptionString()}")
            .AddClass($"mud-elevation-{Elevation}", Elevation > 0)
            .AddClass(Class)
            .Build();

    [Parameter]
    public bool Fluid { get; set; }

    [Parameter]
    public string? Src { get; set; }

    [Parameter]
    public string? FallbackSrc { get; set; }

    [Parameter]
    public string? Alt { get; set; }

    [Parameter]
    public int? Height { get; set; }

    [Parameter]
    public int? Width { get; set; }

    [Parameter]
    public int Elevation { set; get; }

    [Parameter]
    public ObjectFit ObjectFit { set; get; } = ObjectFit.Fill;

    [Parameter]
    public ObjectPosition ObjectPosition { set; get; } = ObjectPosition.Center;
}