using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{
    Task<RefreshToken?> GetByTokenAsync(string Token);
}

