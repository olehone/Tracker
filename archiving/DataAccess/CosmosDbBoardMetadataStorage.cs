using DataAccess.Abstractions;
using Domain.Entities;
using Domain.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DataAccess;

internal class CosmosDbBoardMetadataStorage : IBoardMetadataStorage
{
    private readonly Container _container;

    public CosmosDbBoardMetadataStorage(CosmosClient cosmosClient, IOptions<CosmosDbOptions> options)
    {
        _container = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.BoardArchiveLogsContainer);
    }

    public async Task<BoardMetadata?> GetAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<BoardMetadata>(
                id: boardId.ToString(),
                partitionKey: new PartitionKey(boardId.ToString()),
                cancellationToken: cancellationToken);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<BoardMetadata> CreateAsync(BoardMetadata boardArchiveLog, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            item: boardArchiveLog,
            partitionKey: new PartitionKey(boardArchiveLog.BoardId.ToString()),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<BoardMetadata> UpdateAsync(BoardMetadata boardArchiveLog, CancellationToken cancellationToken = default)
    {
        var response = await _container.ReplaceItemAsync(
            item: boardArchiveLog,
            id: boardArchiveLog.BoardId.ToString(),
            partitionKey: new PartitionKey(boardArchiveLog.BoardId.ToString()),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task AppendLogAsync(Guid boardId, ArchiveLog log, CancellationToken cancellationToken = default)
    {
        var patchOperations = new List<PatchOperation>
        {
            PatchOperation.Add("/logs/-", log),
            PatchOperation.Set("/lastLog", log)
        };

        await _container.PatchItemAsync<BoardMetadata>(
            id: boardId.ToString(),
            partitionKey: new PartitionKey(boardId.ToString()),
            patchOperations: patchOperations,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        await _container.DeleteItemAsync<BoardMetadata>(
            id: boardId.ToString(),
            partitionKey: new PartitionKey(boardId.ToString()),
            cancellationToken: cancellationToken);
    }
}