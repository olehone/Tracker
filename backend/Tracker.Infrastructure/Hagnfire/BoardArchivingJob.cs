using Tracker.Application.Common.Jobs;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;

namespace Tracker.Infrastructure.Hagnfire;

internal class BoardArchivingJob(IUnitOfWorkFactory unitOfWorkFactory) : IBoardArchivingJob
{
    public async Task ExecuteAsync()
    {
        await using var uow = unitOfWorkFactory.Create();

        var boards = await uow.BoardRepository.GetByArchiveStatusAsync(ArchiveStatus.Pending);

        foreach(var board in boards)
        {
            Console.WriteLine($"Pick up board {board.Id} {board.Title} for archiving");
            board.ArchiveStatus = ArchiveStatus.Queued;
            uow.BoardRepository.Update(board);
        }

        await uow.SaveChangesAsync();
    }
}
