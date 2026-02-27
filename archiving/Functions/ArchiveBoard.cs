using Azure.Messaging.ServiceBus;
using DataAccess.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions;

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
        await messageActions.CompleteMessageAsync(message);
    }
}