using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Soenneker.Blazor.Drawflow;
using Soenneker.Blazor.Drawflow.Dtos;
using Soenneker.Blazor.Drawflow.Options;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests.Roadmap;
using Tracker.Services.Abstraction.Board;
using Tracker.WebApp.Components.Items;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class Roadmap : BoardSubscribeBase
{
    // ── Injected ────────────────────────────────────────────────────────────
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IRoadmapService RoadmapService { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;

    // ── Drawflow ─────────────────────────────────────────────────────────────
    private Drawflow? _flow;
    private readonly DrawflowOptions _options = new()
    {
        Reroute = false,
        RerouteFixCurvature = false,
        Curvature = 0.5,
        ForceFirstInput = false,
        EditorMode = "edit"
    };

    // ── State ────────────────────────────────────────────────────────────────
    // Maps drawflow integer ID ↔ BoardItem Guid
    private readonly Dictionary<int, Guid> _nodeMap = [];
    private readonly Dictionary<Guid, int> _reverseMap = [];
    private int _nextNodeId = 1;

    private bool _canvasLoaded;
    private bool _saving;
    private CancellationTokenSource? _saveCts;

    // ── Lifecycle ────────────────────────────────────────────────────────────
    protected override void OnInitialized()
    {
        base.OnInitialized();
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_canvasLoaded)
        {
            _canvasLoaded = true;
            await LoadCanvas();
        }
    }

    protected override void StateHasChangedHandler()
    {
        _ = SyncNewItemsAsync();
        base.StateHasChangedHandler();
    }

    public override void Dispose()
    {
        base.Dispose();
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
        _saveCts?.Cancel();
    }

    // ── Canvas load ──────────────────────────────────────────────────────────
    private async Task LoadCanvas()
    {
        if (_flow is null)
            return;

        // FIX: was LoadAsync — correct method name is GetAsync per IRoadmapService
        var result = await RoadmapService.GetAsync(BoardState.Board!.Id);
        if (result.IsFailure)
            return;

        var dto = result.Value;
        var items = BoardState.ItemsState.BoardItems;

        _nodeMap.Clear();
        _reverseMap.Clear();
        _nextNodeId = 1;

        if (dto.Nodes.Count == 0)
            await AutoPlaceItemsAsync(items);
        else
            await ImportSavedCanvasAsync(dto, items);
    }

    private async Task AutoPlaceItemsAsync(IEnumerable<BoardItemDto> items)
    {
        const int cols = 5;
        const int xStep = 240;
        const int yStep = 140;
        int col = 0, row = 0;

        foreach (var item in items)
        {
            int x = 40 + col * xStep;
            int y = 40 + row * yStep;
            await AddNodeAsync(item, x, y);

            col++;
            if (col >= cols)
            { col = 0; row++; }
        }
    }

    private async Task ImportSavedCanvasAsync(RoadmapDto dto, IEnumerable<BoardItemDto> items)
    {
        var itemLookup = items.ToDictionary(i => i.Id);

        // Assign sequential drawflow IDs to saved nodes
        var nodeDrawflowId = new Dictionary<Guid, int>(); // RoadmapNode.Id → drawflow int

        foreach (var node in dto.Nodes)
        {
            if (!itemLookup.ContainsKey(node.BoardItemId))
                continue; // item was deleted

            int id = _nextNodeId++;
            _nodeMap[id] = node.BoardItemId;
            _reverseMap[node.BoardItemId] = id;
            nodeDrawflowId[node.Id] = id;
        }

        // FIX: was BuildImportJson + ImportAsJson (neither exists in v4).
        // Build a DrawflowExport object and call Import() instead.
        var moduleData = new Dictionary<string, DrawflowNode>();

        foreach (var node in dto.Nodes)
        {
            if (!itemLookup.TryGetValue(node.BoardItemId, out var item))
                continue;
            if (!nodeDrawflowId.TryGetValue(node.Id, out int dfId))
                continue;

            var outgoing = dto.Arrows
                .Where(a => a.SourceNodeId == node.Id && nodeDrawflowId.ContainsKey(a.TargetNodeId))
                .Select(a => new DrawflowConnection
                {
                    Node = nodeDrawflowId[a.TargetNodeId].ToString(),
                    Input = "input_1"
                })
                .ToList();

            var incoming = dto.Arrows
                .Where(a => a.TargetNodeId == node.Id && nodeDrawflowId.ContainsKey(a.SourceNodeId))
                .Select(a => new DrawflowConnection
                {
                    Node = nodeDrawflowId[a.SourceNodeId].ToString(),
                    Input = "output_1"
                })
                .ToList();

            moduleData[dfId.ToString()] = new DrawflowNode
            {
                Id = dfId.ToString(),
                Name = "task",
                Data = new Dictionary<string, object> { ["boardItemId"] = item.Id.ToString() },
                Class = "roadmap-node",
                Html = BuildNodeHtml(item),
                PosX = (int)node.X,
                PosY = (int)node.Y,
                Inputs = new Dictionary<string, DrawflowNodeIO>
                {
                    ["input_1"] = new DrawflowNodeIO { Connections = incoming }
                },
                Outputs = new Dictionary<string, DrawflowNodeIO>
                {
                    ["output_1"] = new DrawflowNodeIO { Connections = outgoing }
                }
            };
        }

        var exportData = new DrawflowExport
        {
            Drawflow = new Dictionary<string, DrawflowModule>
            {
                ["Home"] = new DrawflowModule { Data = moduleData }
            }
        };

        await _flow!.Import(exportData);

        // Add nodes for any items that exist but have no saved position yet
        var positionedItems = dto.Nodes.Select(n => n.BoardItemId).ToHashSet();
        var unpositioned = items.Where(i => !positionedItems.Contains(i.Id)).ToList();
        if (unpositioned.Count > 0)
        {
            int x = 40, y = 40 + ((_nodeMap.Count / 5) + 1) * 140;
            foreach (var item in unpositioned)
            {
                await AddNodeAsync(item, x, y);
                x += 240;
            }
        }
    }

    // ── Node management ──────────────────────────────────────────────────────
    private async Task AddNodeAsync(BoardItemDto item, double x, double y)
    {
        if (_flow is null || _reverseMap.ContainsKey(item.Id))
            return;

        int id = _nextNodeId++;
        _nodeMap[id] = item.Id;
        _reverseMap[item.Id] = id;

        // FIX: was using named parameter cssClass: which does not exist in v4 — use positional args
        await _flow.AddNode(
            "task",
            1,
            1,
            (int)x,
            (int)y,
            "roadmap-node",
            new { boardItemId = item.Id.ToString() },
            BuildNodeHtml(item));
    }

    private async Task SyncNewItemsAsync()
    {
        if (!_canvasLoaded || _flow is null)
            return;

        var allItems = BoardState.ItemsState.BoardItems;
        int col = 0;
        double x = 40, y = 40;

        foreach (var item in allItems.Where(i => !_reverseMap.ContainsKey(i.Id)))
        {
            await AddNodeAsync(item, x + col * 240, y);
            col++;
        }
    }

    // ── Node HTML ────────────────────────────────────────────────────────────
    private static string BuildNodeHtml(BoardItemDto item)
    {
        var (importanceColor, importanceLabel) = item.Importance switch
        {
            BoardItemImportance.High => ("#e65100", "High"),
            BoardItemImportance.Medium => ("#1565c0", "Medium"),
            _ => ("#546e7a", "Low")
        };

        var dueText = item.DueDate.HasValue
            ? $"<span class='rn-due'>{item.DueDate.Value.LocalDateTime:MMM d}</span>"
            : string.Empty;

        var doneStyle = item.IsDone ? "text-decoration:line-through;opacity:0.6;" : string.Empty;

        // FIX: was $$$"""...""" (triple-$ raw string) — single $ is sufficient here
        // since the template contains no literal { } that need escaping
        return $"""
            <div class="rn-card {(item.IsDone ? "rn-done" : string.Empty)}">
              <div class="rn-importance" style="background:{importanceColor}" title="{importanceLabel}"></div>
              <div class="rn-body">
                <div class="rn-title" style="{doneStyle}">{System.Net.WebUtility.HtmlEncode(item.Title)}</div>
                <div class="rn-meta">
                  {dueText}
                </div>
              </div>
            </div>
            """;
    }

    // ── Drawflow event handlers ──────────────────────────────────────────────
    private Task OnNodeSelected(List<string> drawflowIds)
    {
        if (drawflowIds == null || drawflowIds.Count == 0)
            return Task.CompletedTask;

        // If only one selection matters:
        var drawflowId = drawflowIds.First();

        if (!int.TryParse(drawflowId, out int id))
            return Task.CompletedTask;

        if (!_nodeMap.TryGetValue(id, out var boardItemId))
            return Task.CompletedTask;

        var item = BoardState.ItemsState.BoardItems
            .FirstOrDefault(i => i.Id == boardItemId);

        if (item is null)
            return Task.CompletedTask;

        return OpenItemSettingsAsync(item);
    }

    private Task OnConnectionCreated(string _) => DebounceSaveAsync();
    private Task OnConnectionRemoved(string _) => DebounceSaveAsync();
    private Task OnNodeMoved(string _) => DebounceSaveAsync();

    // ── Save ─────────────────────────────────────────────────────────────────
    private async Task DebounceSaveAsync()
    {
        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();
        var token = _saveCts.Token;

        try
        {
            await Task.Delay(800, token);
            await SaveAsync();
        }
        catch (TaskCanceledException) { }
    }

    private async Task SaveAsync()
    {
        if (_flow is null)
            return;

        _saving = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            var export = await _flow.Export();

            // FIX: was export.Drawflow.Home.Data — Drawflow is Dictionary<string, DrawflowModule>,
            // so Home must be accessed via the indexer, not a property.
            if (export?.Drawflow is null || !export.Drawflow.TryGetValue("Home", out var homeModule) || homeModule.Data is null)
                return;

            var nodes = new List<SaveRoadmapNodeRequest>();
            var arrows = new List<SaveRoadmapArrowRequest>();

            // FIX: was foreach (var (idStr, node) in ...) — type inference fails on
            // Dictionary<string, DrawflowNode>; use explicit KeyValuePair iteration instead
            foreach (KeyValuePair<string, DrawflowNode> pair in homeModule.Data)
            {
                if (!int.TryParse(pair.Key, out int dfId))
                    continue;
                if (!_nodeMap.TryGetValue(dfId, out var itemId))
                    continue;

                nodes.Add(new SaveRoadmapNodeRequest
                {
                    BoardItemId = itemId,
                    X = pair.Value.PosX,
                    Y = pair.Value.PosY
                });

                if (pair.Value.Outputs?.TryGetValue("output_1", out var output) == true)
                {
                    foreach (var conn in output.Connections ?? [])
                    {
                        if (!int.TryParse(conn.Node, out int targetDfId))
                            continue;
                        if (!_nodeMap.TryGetValue(targetDfId, out var targetItemId))
                            continue;

                        arrows.Add(new SaveRoadmapArrowRequest
                        {
                            SourceBoardItemId = itemId,
                            TargetBoardItemId = targetItemId
                        });
                    }
                }
            }

            await RoadmapService.SaveAsync(BoardState.Board!.Id, new SaveRoadmapRequest
            {
                Nodes = nodes,
                Arrows = arrows
            });
        }
        finally
        {
            _saving = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    // ── Toolbar actions ──────────────────────────────────────────────────────
    // FIX: was returning ValueTask — MudBlazor OnClick is EventCallback<MouseEventArgs>,
    // which requires Task (not ValueTask). Accept MouseEventArgs and wrap ValueTask → Task.
    private Task ZoomIn(MouseEventArgs _) => _flow?.ZoomIn().AsTask() ?? Task.CompletedTask;
    private Task ZoomOut(MouseEventArgs _) => _flow?.ZoomOut().AsTask() ?? Task.CompletedTask;

    // FIX: was _flow.Zoom(1) — no such method in v4; drawflow.js has no auto-fit API.
    // A workaround is to reset via ZoomIn/ZoomOut or leave as no-op.
    private Task FitView(MouseEventArgs _) => Task.CompletedTask;

    // ── Open item dialog ─────────────────────────────────────────────────────
    private async Task OpenItemSettingsAsync(BoardItemDto item)
    {
        var parameters = new DialogParameters
        {
            { nameof(ItemSettingsDialog.BoardState), BoardState },
            { nameof(ItemSettingsDialog.Item), item }
        };

        var dialog = await DialogService.ShowAsync<ItemSettingsDialog>(
            item.Title,
            parameters,
            new DialogOptions { CloseButton = false, NoHeader = true, MaxWidth = MaxWidth.Large });

        await dialog.Result;
    }
}
