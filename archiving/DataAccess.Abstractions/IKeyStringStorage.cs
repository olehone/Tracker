namespace DataAccess.Abstractions;

public interface IKeyStringStorage
{
    Task DeleteAsync(Guid id, CancellationToken cancelationToken = default);
    Task<string?> GetAsync(Guid id, CancellationToken cancelationToken = default);
    Task PutAsync(Guid id, string data, CancellationToken cancelationToken = default);
}