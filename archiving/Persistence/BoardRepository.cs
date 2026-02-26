using ArchivingFunction.Domain.Entities;
using ArchivingFunction.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace ArchivingFunction.Persistence;

public class BoardRepository(ApplicationDbContext dbContext)
    : IBoardRepository
{
    public async Task<Board?> LoadFullBoardAsync(Guid boardId, CancellationToken ct = default)
    {
        return await dbContext.Boards
            .AsNoTracking()
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Assignees)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Comments)
                        .ThenInclude(c => c.Attachments)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Attachments)
            .FirstOrDefaultAsync(b => b.Id == boardId, ct);
    }

    public async Task DeleteBoardContentAsync(Guid boardId, CancellationToken ct = default)
    {
        var board = await dbContext.Boards
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Assignees)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Comments)
                        .ThenInclude(c => c.Attachments)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Attachments)
            .FirstOrDefaultAsync(b => b.Id == boardId, ct);

        if (board is null)
        {
            return;
        }

        dbContext.RemoveRange(board.BoardLists);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RestoreBoardContentAsync(Board snapshot, CancellationToken ct = default)
    {
        dbContext.BoardLists.AddRange(snapshot.BoardLists);
        await dbContext.SaveChangesAsync(ct);
    }
}