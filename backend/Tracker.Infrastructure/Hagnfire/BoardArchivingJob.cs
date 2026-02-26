using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Jobs;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Hagnfire;

internal class BoardArchivingJob(IUnitOfWorkFactory unitOfWorkFactory,
    ServiceBusClient serviceBus,
    IOptions<ServiceBusOptions> serviceBusOptions) : IBoardArchivingJob
{
    public async Task ExecuteAsync()
    {
        await using var uow = unitOfWorkFactory.Create();

        var boards = await uow.BoardRepository.GetByArchiveStatusAsync(ArchiveStatus.Pending);

        if (boards.Count == 0)
        {
            return;
        }

        var queue = serviceBusOptions.Value.BoardArchiveQueueName;
        var subject = serviceBusOptions.Value.BoardArchiveSubjectName;

        await using var sender = serviceBus.CreateSender(queue);

        var batch = await sender.CreateMessageBatchAsync();
        try
        {
            await SendMessages(uow, boards, subject, sender, batch);
        }
        finally
        {
            batch.Dispose();
        }
    }

    private static async Task SendMessages(IUnitOfWork uow,
        IReadOnlyList<Board> boards,
        string subject,
        ServiceBusSender sender,
        ServiceBusMessageBatch batch)
    {
        foreach (var board in boards)
        {
            var message = new ServiceBusMessage(board.Id.ToString())
            {
                MessageId = board.Id.ToString(),
                Subject = subject
            };

            if (!batch.TryAddMessage(message))
            {
                await sender.SendMessagesAsync(batch);
                batch.Dispose();

                var newBatch = await sender.CreateMessageBatchAsync();
                batch = newBatch;
                if (!batch.TryAddMessage(message))
                {
                    throw new Exception("Message is too large");
                }
            }
            board.ArchiveStatus = ArchiveStatus.Queued;
            uow.BoardRepository.Update(board);
        }

        if (batch.Count > 0)
        {
            await sender.SendMessagesAsync(batch);
        }

        await uow.SaveChangesAsync();
    }
}