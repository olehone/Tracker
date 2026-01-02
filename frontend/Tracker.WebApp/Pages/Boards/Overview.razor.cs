using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages.Boards;

public partial class Overview
{
    [Parameter]
    public Guid BoardId { get; set; }

    [Inject]
    private IBoardService BoardService { get; set; } = null!;
    [Inject]
    private IBoardItemService BoardItemService { get; set; } = null!;
    [Inject]
    private IBoardListService BoardListService { get; set; } = null!;
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    public BoardFullDto? Board { get; set; }

    private MudDropContainer<BoardItemDto> _container = null!;
    private List<BoardItemDto> _items = null!;

    protected override async Task OnInitializedAsync()
    {
        Board = await BoardService.GetBoardByIdAsync(BoardId);
        _items = Board.BoardLists.SelectMany(bl => bl.BoardItems).ToList();
        StateHasChanged();
    }

    private void ItemDropped(MudItemDropInfo<BoardItemDto> item)
    {
        if (item.Item is null)
        {
            return;
        }

        var request = new MoveBoardItemRequest()
        {
            ToBoardListId = Guid.Parse(item.DropzoneIdentifier),
            BoardItemId = item.Item.Id,
            Position = item.IndexInZone + 1
        };

        RestructureItems(request);
        _ = Task.Run(async () =>
        {
            try
            {
                await BoardItemService.MoveBoardItemAsync(request);
            }
            catch (Exception)
            {
                Navigation.Refresh();
            }
        });
    }

    private void RestructureItems(MoveBoardItemRequest request)
    {
        if (Board is null)
        {
            return;
        }
        var item = Board.BoardLists
            .SelectMany(bl => bl.BoardItems)
            .FirstOrDefault(bi => bi.Id == request.BoardItemId);
        if (item is null)
        {
            return;
        }

        var fromBoardList = Board.BoardLists.FirstOrDefault(bl => bl.Id == item.BoardListId);
        if (fromBoardList is null)
        {
            return;
        }

        if (item.BoardListId == request.ToBoardListId)
        {
            if (item.Position == request.Position)
            {
                return;
            }

            if (item.Position > request.Position)
            {
                ShiftItemsPosition(fromBoardList, +1, request.Position, item.Position - 1);
                item.Position = request.Position;
            }
            else
            {
                ShiftItemsPosition(fromBoardList, -1, item.Position + 1, request.Position);
                item.Position = request.Position;
            }
        }
        else
        {
            var toBoardList = Board.BoardLists.FirstOrDefault(bl => bl.Id == request.ToBoardListId);
            if (toBoardList is null)
            {
                return;
            }

            fromBoardList.BoardItems.Remove(item);
            ShiftItemsPosition(fromBoardList, -1, item.Position);
            ShiftItemsPosition(toBoardList, +1, request.Position);
            item.BoardListId = request.ToBoardListId;
            item.Position = request.Position;
            toBoardList.BoardItems.Insert(item.Position - 1, item);
        }

        StateHasChanged();
    }


    private async Task CreateList(string title)
    {
        if (Board is null)
        {
            return;
        }
        var request = new CreateBoardListRequest()
        {
            BoardId = Board.Id,
            Title = title
        };
        var list = await BoardListService.CreateBoardListAsync(request);
        Board.BoardLists.Add(list);
        StateHasChanged();
    }

    private async Task CreateItemForList(BoardListDto list, string title)
    {
        var request = new CreateBoardItemRequest
        {
            BoardListId = list.Id,
            Title = title
        };
        var item = await BoardItemService.CreateBoardItemAsync(request);
        list.BoardItems.Add(item);
        _items.Add(item);
        _container.Refresh();
        StateHasChanged();
    }


    private void ShiftItemsPosition(BoardListDto list, int delta, int from)
    {
        if (Board is null)
        {
            return;
        }

        if (list is null)
        {
            return;
        }

        foreach (var item in list.BoardItems.Where(bi => bi.Position >= from))
        {
            item.Position += delta;
        }
    }

    private void ShiftItemsPosition(BoardListDto list, int delta, int from, int to)
    {
        if (Board is null)
        {
            return;
        }

        if (list is null)
        {
            return;
        }

        foreach (var item in list.BoardItems.Where(bi => bi.Position >= from && bi.Position <= to))
        {
            item.Position += delta;
        }
    }

}