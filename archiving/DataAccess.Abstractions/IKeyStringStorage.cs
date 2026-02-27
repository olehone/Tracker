namespace DataAccess.Abstractions;

public interface IKeyStringStorage
{
    Task DeleteAsync(string fileName, CancellationToken cancelationToken = default);
    Task<string?> GetAsync(string fileName, CancellationToken cancelationToken = default);
    Task PutAsync(string fileName, string data, CancellationToken cancelationToken = default);
}