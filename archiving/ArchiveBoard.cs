using System.Text.Json;
using ArchivingFunction.Domain.Enums;
using ArchivingFunction.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ArchivingFunction;

public class ArchiveBoard(ILogger<ArchiveBoard> logger,
    IBoardRepository boardRepository)
{
    private const string ArchiveQueueName = "archive-queue";

    [Function(nameof(ArchiveBoard))]
    public async Task Run(
        [ServiceBusTrigger(ArchiveQueueName, Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        logger.LogInformation("Archive board {id}", message.Body);

        var payload = message.Body.ToObjectFromJson<ArchiveBoardMessage>();
        if (payload is null)
        {
            logger.LogCritical("Cannot deserialize archive board message payload {id} - {payload}",
                message.MessageId, message.Body);
            return;
        }

        var board = await boardRepository.LoadFullBoardAsync(payload.BoardId);

        if (board is null)
        {
            logger.LogInformation("Board {id} is not found", payload.BoardId);
            return;
        }

        if (board.ArchiveStatus != ArchiveStatus.Queued)
        {
            logger.LogInformation("Board {id} is not queued anymore", board.Id);
            return;
        }

        var serializedBoard = JsonSerializer.Serialize(board);

        logger.LogInformation("Serialized board {data}", serializedBoard);

        boardRepository.UpdateBoardArchiveStatusAsync(board.Id, ArchiveStatus.Completed);
        await boardRepository.SaveChangesAsync();
        await messageActions.CompleteMessageAsync(message);
    }
}