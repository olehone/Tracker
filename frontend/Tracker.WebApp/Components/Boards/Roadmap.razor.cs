using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests.Roadmap;
using Tracker.Services.Abstraction.Board;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class Roadmap : IDisposable
{
    // ── Parameters & injections ────────────────────────────────────────

    [CascadingParameter] private BoardState BoardState { get; set; } = null!;
    [Inject] private IRoadmapService RoadmapService { get; set; } = null!;

    // ── State ──────────────────────────────────────────────────────────

    private readonly BlazorDiagram _diagram = new();
    private bool _loading = true;
    private bool _saving;
    private bool _saved;
    private bool _isLinking;          // true while user is drawing a link

    // Approximate rendered size of one node card.
    // The library uses this immediately to place ports correctly before
    // ResizeObserver fires. Height adjusts automatically after first render.
    private static readonly Size NodeSize = new(280, 48);

    // ── Lifecycle ──────────────────────────────────────────────────────
    private Guid BoardId => BoardState.Board.Id;

    protected override void OnInitialized()
    {
        _diagram.RegisterComponent<RoadmapItemNode, RoadmapNodeWidget>();

        // Track when the user starts/finishes drawing a link so the
        // Roadmap container can add .rn-diagram-linking and reveal all ports.
        _diagram.Links.Added += OnLinkAdded;
        _diagram.Links.Removed += OnLinkRemoved;
    }

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        await LoadAsync();
        _loading = false;
    }

    public void Dispose()
    {
        _diagram.Links.Added -= OnLinkAdded;
        _diagram.Links.Removed -= OnLinkRemoved;
        if (_diagram is IDisposable d)
            d.Dispose();
    }

    // ── Link drag tracking ─────────────────────────────────────────────

    private void OnLinkAdded(BaseLinkModel link)
    {
        // A user-drawn link starts with no target model (just a position anchor).
        // Saved/loaded links have both ends set at creation time.
        if (link.Target?.Model is null)
        {
            _isLinking = true;
            link.TargetChanged += OnLinkCompleted;
            InvokeAsync(StateHasChanged);
        }
    }

    private void OnLinkCompleted(BaseLinkModel link, Anchor? oldAnchor, Anchor? newAnchor)
    {
        link.TargetChanged -= OnLinkCompleted;
        _isLinking = false;
        InvokeAsync(StateHasChanged);
    }

    private void OnLinkRemoved(BaseLinkModel link)
    {
        // Handles the case where the user cancels mid-drag (link removed without target).
        if (!_diagram.Links.Any(l => l.Target?.Model is null))
        {
            _isLinking = false;
            InvokeAsync(StateHasChanged);
        }
    }

    // ── Load ───────────────────────────────────────────────────────────

    private async Task LoadAsync()
    {
        var result = await RoadmapService.GetAsync(BoardId);
        if (!result.IsSuccess)
            return;

        var roadmap = result.Value;
        _diagram.Nodes.Clear();
        _diagram.Links.Clear();

        var nodeDtoById = roadmap.Nodes.ToDictionary(n => n.Id);
        var nodeDtoByBoardItemId = roadmap.Nodes.ToDictionary(n => n.BoardItemId);

        var diagramNodeByItemId = new Dictionary<Guid, RoadmapItemNode>();
        var col = 0;

        foreach (var item in BoardState.ItemsState.BoardItems)
        {
            var position = nodeDtoByBoardItemId.TryGetValue(item.Id, out var saved)
                ? new Point(saved.X, saved.Y)
                : new Point(col * 320, 40);
            col++;

            var node = new RoadmapItemNode(item, position)
            {
                // Give the library the node dimensions immediately so it can
                // compute port positions before ResizeObserver fires.
                Size = NodeSize
            };

            _diagram.Nodes.Add(node);
            diagramNodeByItemId[item.Id] = node;
        }

        foreach (var arrow in roadmap.Arrows)
        {
            if (!nodeDtoById.TryGetValue(arrow.SourceNodeId, out var srcDto))
                continue;
            if (!nodeDtoById.TryGetValue(arrow.TargetNodeId, out var tgtDto))
                continue;
            if (!diagramNodeByItemId.TryGetValue(srcDto.BoardItemId, out var srcNode))
                continue;
            if (!diagramNodeByItemId.TryGetValue(tgtDto.BoardItemId, out var tgtNode))
                continue;

            var srcPort = GetPort(srcNode, GetFromEnum(arrow.SourceSide));
            var tgtPort = GetPort(tgtNode, GetFromEnum(arrow.TargetSide));

            _diagram.Links.Add(new LinkModel(srcPort, tgtPort));
        }
    }

    // ── Save ───────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        _saving = true;
        _saved = false;
        StateHasChanged();

        var request = new SaveRoadmapRequest
        {
            Nodes = _diagram.Nodes
                .OfType<RoadmapItemNode>()
                .Select(n => new SaveRoadmapNodeRequest
                {
                    BoardItemId = n.Item.Id,
                    X = n.Position.X,
                    Y = n.Position.Y
                })
                .ToList(),

            Arrows = _diagram.Links
                .OfType<LinkModel>()
                .Select(TryGetArrowRequest)
                .OfType<SaveRoadmapArrowRequest>()
                .ToList()
        };

        await RoadmapService.SaveAsync(BoardId, request);

        _saving = false;
        _saved = true;
        StateHasChanged();
    }

    private static SaveRoadmapArrowRequest? TryGetArrowRequest(LinkModel link)
    {
        if (link.Source.Model is not PortModel srcPort)
            return null;
        if (link.Target?.Model is not PortModel tgtPort)
            return null;
        if (srcPort.Parent is not RoadmapItemNode srcNode)
            return null;
        if (tgtPort.Parent is not RoadmapItemNode tgtNode)
            return null;

        return new SaveRoadmapArrowRequest
        {
            SourceBoardItemId = srcNode.Item.Id,
            TargetBoardItemId = tgtNode.Item.Id,

            SourceSide = GetSideEnum(srcPort.Alignment),
            TargetSide = GetSideEnum(tgtPort.Alignment)
        };
    }

    private PortAlignment GetFromEnum(SideEnum side)
    {
        return side switch
        {
            SideEnum.Top => PortAlignment.Top,
            SideEnum.TopRight => PortAlignment.TopRight,
            SideEnum.Right => PortAlignment.Right,
            SideEnum.BottomRight => PortAlignment.BottomRight,
            SideEnum.Bottom => PortAlignment.Bottom,
            SideEnum.BottomLeft => PortAlignment.BottomLeft,
            SideEnum.Left => PortAlignment.Left,
            SideEnum.TopLeft => PortAlignment.TopLeft,
            _ => PortAlignment.TopLeft
        };
    }

    private static SideEnum GetSideEnum(PortAlignment alignment)
    {
        return alignment switch
        {
            PortAlignment.Top => SideEnum.Top,
            PortAlignment.TopRight => SideEnum.TopRight,
            PortAlignment.Right => SideEnum.Right,
            PortAlignment.BottomRight => SideEnum.BottomRight,
            PortAlignment.Bottom => SideEnum.Bottom,
            PortAlignment.BottomLeft => SideEnum.BottomLeft,
            PortAlignment.Left => SideEnum.Left,
            PortAlignment.TopLeft => SideEnum.TopLeft,
            _ => SideEnum.Left
        };
    }

    private static PortModel GetPort(RoadmapItemNode node, PortAlignment alignment)
    {
        return alignment switch
        {
            PortAlignment.Top => node.TopPort,
            PortAlignment.Bottom => node.BottomPort,
            PortAlignment.Left => node.LeftPort,
            PortAlignment.Right => node.RightPort,
            _ => node.RightPort
        };
    }
}
