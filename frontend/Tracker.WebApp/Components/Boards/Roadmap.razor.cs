using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Microsoft.AspNetCore.Components;
using Tracker.Domain.Requests.Roadmap;
using Tracker.Services.Abstraction.Board;
using Tracker.WebApp.Components.Roadmap;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class Roadmap
{
    // ── Parameters & injections ────────────────────────────────────────────

    [Parameter] public Guid BoardId { get; set; }

    /// <summary>
    /// Provided by the parent board layout so ItemBrief works normally.
    /// Also cascaded implicitly into every child widget rendered inside DiagramCanvas.
    /// </summary>
    [CascadingParameter] private BoardState BoardState { get; set; } = null!;

    [Inject] private IRoadmapService RoadmapService { get; set; } = null!;

    // ── State ──────────────────────────────────────────────────────────────

    private readonly BlazorDiagram _diagram = new();
    private bool _loading = true;
    private bool _saving;

    // ── Lifecycle ──────────────────────────────────────────────────────────


    protected override async Task OnInitializedAsync()
    {
        // Register the custom node → widget mapping once.
        _diagram.RegisterComponent<RoadmapItemNode, RoadmapNodeWidget>();
    }

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        await LoadAsync();
        _loading = false;
    }

    public void Dispose()
    {
        // BlazorDiagram doesn't implement IDisposable in all versions;
        // guard with a pattern-match so it compiles either way.
        if (_diagram is IDisposable d)
            d.Dispose();
    }

    // ── Load ───────────────────────────────────────────────────────────────

    private async Task LoadAsync()
    {
        var result = await RoadmapService.GetAsync(BoardId);
        if (!result.IsSuccess)
            return;

        var roadmap = result.Value;

        _diagram.Nodes.Clear();
        _diagram.Links.Clear();

        // Build lookup tables from the persisted roadmap.
        // RoadmapNodeDto.Id  → dto    (for resolving arrow endpoints)
        // RoadmapNodeDto.BoardItemId → dto  (for looking up saved positions)
        var nodeDtoById = roadmap.Nodes.ToDictionary(n => n.Id);
        var nodeDtoByBoardItemId = roadmap.Nodes.ToDictionary(n => n.BoardItemId);

        // One diagram node per board item (new items land on a grid row).
        var diagramNodeByItemId = new Dictionary<Guid, RoadmapItemNode>();
        var col = 0;

        foreach (var item in BoardState.ItemsState.BoardItems)
        {
            var position = nodeDtoByBoardItemId.TryGetValue(item.Id, out var saved)
                ? new Point(saved.X, saved.Y)
                : new Point(col * 280, 20);   // default grid position for new items

            col++;

            var node = new RoadmapItemNode(item, position);
            _diagram.Nodes.Add(node);
            diagramNodeByItemId[item.Id] = node;
        }

        // Restore persisted arrows.
        // Arrow DTO uses node IDs, so resolve via the dto lookup first.
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

            _diagram.Links.Add(new LinkModel(
                srcNode.GetPort(PortAlignment.Right),
                tgtNode.GetPort(PortAlignment.Left)));
        }
    }

    // ── Save ───────────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        _saving = true;
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
                .Select(link => TryGetArrowRequest(link))
                .OfType<SaveRoadmapArrowRequest>()   // filters out nulls
                .ToList()
        };

        await RoadmapService.SaveAsync(BoardId, request);

        _saving = false;
    }

    /// <summary>
    /// Converts a diagram link to a save request, or returns null if the link
    /// endpoints are not both RoadmapItemNodes (e.g. a dangling link being drawn).
    /// </summary>
    private static SaveRoadmapArrowRequest? TryGetArrowRequest(LinkModel link)
    {
        if (link.Source.Model is not PortModel srcPort)
            return null;
        if (link.Target.Model is not PortModel tgtPort)
            return null;
        if (srcPort.Parent is not RoadmapItemNode srcNode)
            return null;
        if (tgtPort.Parent is not RoadmapItemNode tgtNode)
            return null;

        return new SaveRoadmapArrowRequest
        {
            SourceBoardItemId = srcNode.Item.Id,
            TargetBoardItemId = tgtNode.Item.Id
        };
    }
}
