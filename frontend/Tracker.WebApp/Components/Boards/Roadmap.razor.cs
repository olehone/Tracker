using Microsoft.AspNetCore.Components;
using MudBlazor;
using Soenneker.Blazor.Drawflow;
using Soenneker.Blazor.Drawflow.Options;
using System.Text;
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
        // When new items arrive (real-time or after a create), add any missing nodes
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

        var dto = await RoadmapService.LoadAsync(BoardState.Board!.Id);
        var items = BoardState.ItemsState.BoardItems;

        _nodeMap.Clear();
        _reverseMap.Clear();
        _nextNodeId = 1;

        if (dto.Nodes.Count == 0)
        {
            // First time — auto-place items in a grid (5 per row)
            await AutoPlaceItemsAsync(items);
        }
        else
        {
            await ImportSavedCanvasAsync(dto, items);
        }
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
            if (!itemLookup.TryGetValue(node.BoardItemId, out var item))
                continue; // item was deleted

            int id = _nextNodeId++;
            _nodeMap[id] = item.Id;
            _reverseMap[item.Id] = id;
            nodeDrawflowId[node.Id] = id;
        }

        // Build drawflow JSON and import in one shot (avoids 1-node-at-a-time async calls)
        var json = BuildImportJson(dto, itemLookup, nodeDrawflowId);
        await _flow!.ImportAsJson(json);

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

        await _flow.AddNode(
            name: "task",
            inputs: 1,
            outputs: 1,
            posX: x,
            posY: y,
            cssClass: "roadmap-node",
            data: new { boardItemId = item.Id.ToString() },
            html: BuildNodeHtml(item));
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

        var listName = string.Empty; // enriched later if needed

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

    // ── Drawflow import JSON builder ─────────────────────────────────────────
    private string BuildImportJson(
        RoadmapDto dto,
        Dictionary<Guid, BoardItemDto> itemLookup,
        Dictionary<Guid, int> nodeDrawflowId)
    {
        var sb = new StringBuilder();
        sb.Append("""{"drawflow":{"Home":{"data":{""");

        bool first = true;
        foreach (var node in dto.Nodes)
        {
            if (!itemLookup.TryGetValue(node.BoardItemId, out var item))
                continue;
            if (!nodeDrawflowId.TryGetValue(node.Id, out int dfId))
                continue;

            // Outgoing connections from this node
            var outgoing = dto.Arrows
                .Where(a => a.SourceNodeId == node.Id)
                .Where(a => nodeDrawflowId.ContainsKey(a.TargetNodeId))
                .Select(a => $$$"""{"node":"{{{nodeDrawflowId[a.TargetNodeId]}}}","output":"input_1"}""");

            // Incoming connections to this node
            var incoming = dto.Arrows
                .Where(a => a.TargetNodeId == node.Id)
                .Where(a => nodeDrawflowId.ContainsKey(a.SourceNodeId))
                .Select(a => $$$"""{"node":"{{{nodeDrawflowId[a.SourceNodeId]}}}","input":"output_1"}""");

            string outConn = string.Join(",", outgoing);
            string inConn = string.Join(",", incoming);

            string html = BuildNodeHtml(item).Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "");

            if (!first)
                sb.Append(',');
            first = false;

            sb.Append($"""
                "{dfId}":{{"id":{dfId},"name":"task","data":{{"boardItemId":"{item.Id}"}},"class":"roadmap-node","html":"{html}","pos_x":{node.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},"pos_y":{node.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)},"inputs":{{"input_1":{{"connections":[{inConn}]}}}},"outputs":{{"output_1":{{"connections":[{outConn}]}}}}}}
                """);
        }

        sb.Append("}}}}}");
        return sb.ToString();
    }

    // ── Drawflow event handlers ──────────────────────────────────────────────
    private Task OnNodeSelected(string drawflowId)
    {
        if (!int.TryParse(drawflowId, out int id))
            return Task.CompletedTask;
        if (!_nodeMap.TryGetValue(id, out var boardItemId))
            return Task.CompletedTask;

        var item = BoardState.ItemsState.BoardItems.FirstOrDefault(i => i.Id == boardItemId);
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
            if (export?.Drawflow?.Home?.Data is null)
                return;

            var nodes = new List<SaveRoadmapNodeRequest>();
            var arrows = new List<SaveRoadmapArrowRequest>();

            foreach (var (idStr, node) in export.Drawflow.Home.Data)
            {
                if (!int.TryParse(idStr, out int dfId))
                    continue;
                if (!_nodeMap.TryGetValue(dfId, out var itemId))
                    continue;

                nodes.Add(new SaveRoadmapNodeRequest
                {
                    BoardItemId = itemId,
                    X = node.PosX,
                    Y = node.PosY
                });

                // Outgoing connections
                if (node.Outputs?.TryGetValue("output_1", out var output) == true)
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
    private ValueTask ZoomIn() => _flow?.ZoomIn() ?? ValueTask.CompletedTask;
    private ValueTask ZoomOut() => _flow?.ZoomOut() ?? ValueTask.CompletedTask;
    private ValueTask FitView() => _flow?.Zoom(1) ?? ValueTask.CompletedTask; // reset to 1:1; drawflow has no auto-fit

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
