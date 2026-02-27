using System.Text.Json;
using DataAccess.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Services.Abstractions;

namespace Services;

internal class BoardArchivingService(IBoardRepository boardRepository,
    IKeyStringStorage dataStorage,
    IBoardMetadataStorage metadataStorage) : IBoardArchivingService
{
    public async Task ArchiveBoardAsync(Guid boardId)
    {
        await EnsureMetadataAsync(boardId);

        var board = await boardRepository.LoadFullBoardAsync(boardId);
        if (board is null)
        {
            await AppendLog(boardId, "Board is not found", true);
            return;
        }

        if (board.ArchiveStatus != ArchiveStatus.QueuedArchive)
        {
            await AppendLog(boardId, "Board have invalid archive status", true, board.ArchiveStatus);
            return;
        }

        var serialized = SerializeBoard(board);
        var fileName = FileName(boardId);
        await dataStorage.PutAsync(fileName, serialized);

        await boardRepository.DeleteBoardContentAsync(boardId);
        await boardRepository.UpdateBoardArchiveStatusAsync(boardId, ArchiveStatus.Archived);

        await AppendLog(boardId, "Board archived successfully", false, ArchiveStatus.Archived);
    }

    public async Task UnarchiveBoardAsync(Guid boardId)
    {
        var boardMetadata = await metadataStorage.GetAsync(boardId);
        if (boardMetadata is null)
        {
            await EnsureMetadataAsync(boardId);
            await AppendLog(boardId, "Board metadata is not found", true);
            return;
        }
        else if (boardMetadata.LastLog.Status != ArchiveStatus.Archived)
        {
            await AppendLog(boardId, "Board have invalid archive status", true);
            return;
        }

        var board = await boardRepository.LoadBoardAsync(boardId);
        if (board is null)
        {
            await AppendLog(boardId, "Board is not found", true);
            return;
        }

        var fileName = FileName(boardMetadata.BoardId);

        var boardArchive = await dataStorage.GetAsync(fileName);
        if (boardArchive is null)
        {
            await AppendLog(boardId, "Board archive is not found", true);
            return;
        }

        var deserialized = DeserializeBoard(boardArchive);
        if (deserialized is null)
        {
            await AppendLog(boardId, "Can't deserialize board", true);
            return;
        }

        await boardRepository.RestoreBoardContent(deserialized);
        await boardRepository.UpdateBoardArchiveStatusAsync(boardId, ArchiveStatus.NotArchived);

        await dataStorage.DeleteAsync(fileName);

        await AppendLog(boardId, "Board unarchived successfully", false, ArchiveStatus.NotArchived);
    }

    private async Task AppendLog(Guid boardId, string description, bool isError, ArchiveStatus? status = null)
    {
        var newLog = new ArchiveLog
        {
            Status = status,
            Description = description,
            IsError = isError
        };
        await metadataStorage.AppendLogAsync(boardId, newLog);
    }

    private async Task<BoardMetadata> EnsureMetadataAsync(Guid boardId)
    {
        var metadata = await metadataStorage.GetAsync(boardId);
        if (metadata is not null)
        {
            return metadata;
        }

        var newLog = new ArchiveLog
        {
            Description = "Start logging"
        };

        var newMetadata = new BoardMetadata
        {
            Id = boardId.ToString(),
            BoardId = boardId,
            LastLog = newLog,
            Logs = [newLog]
        };

        return await metadataStorage.CreateAsync(newMetadata);
    }

    public static string SerializeBoard(Board board)
    {
        return JsonSerializer.Serialize(board);
    }

    public static Board? DeserializeBoard(string boardJson)
    {
        return JsonSerializer.Deserialize<Board>(boardJson);
    }

    public static string FileName(Guid id)
    {
        return $"{id}.json";
    }
}
