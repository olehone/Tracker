using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Functions;

public class ArchiveBoard(ILogger<ArchiveBoard> logger,
    IBoardArchivingService archivingService)
{
    private const string QueueName = "archive-queue";

    [Function(nameof(ArchiveBoard))]
    public async Task Run(
        [ServiceBusTrigger(QueueName, Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage busMessage,
        ServiceBusMessageActions messageActions)
    {
        var message = busMessage.Body.ToObjectFromJson<ArchiveBoardMessage>();
        if (message is null)
        {
            logger.LogCritical("Can't get board info from message {id}", busMessage.MessageId);
            return;
        }
        await messageActions.CompleteMessageAsync(busMessage);
        return;
        logger.LogInformation("Got board {id} to archive", message.BoardId);
        await archivingService.ArchiveBoardAsync(message.BoardId);

    }
}