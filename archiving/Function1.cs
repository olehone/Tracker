using ArchivingFunction.Domain.Options;

using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchivingFunction;

public class Function1(ILogger<Function1> logger)
{
    private const string ArchiveQueueName = "archive-queue";

    //[Function("ArchiveBoard")]
    //public async Task Run(
    //    [ServiceBusTrigger("archive-queue",
    //    Connection = "ServiceBusConnection")] string boardIdStr,
    //    ServiceBusMessageActions messageActions)
    //{
    //    _logger.LogInformation("Board to archive: {Id}", boardIdStr);

    //    // Complete the message
    //    await messageActions.CompleteMessageAsync(message);
    //}

    [Function(nameof(Function1))]
    public async Task Run(
        [ServiceBusTrigger(ArchiveQueueName, Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        logger.LogInformation("Message ID: {id}", message.MessageId);
        logger.LogInformation("Message Body: {body}", message.Body);
        logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}