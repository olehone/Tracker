using System.Text;
using Azure.Storage.Blobs;
using DataAccess.Abstractions;
using Domain.Options;
using Microsoft.Extensions.Options;

namespace DataAccess;

internal class BlobKeyStringStorage : IKeyStringStorage
{
    private readonly BlobContainerClient _container;

    public BlobKeyStringStorage(BlobServiceClient blobServiceClient,
        IOptions<BlobOptions> options)
    {
        var containerName = options.Value.ArchiveContainerName;
        _container = blobServiceClient.GetBlobContainerClient(containerName);
    }

    private async Task EnsureContainerAsync(CancellationToken cancelationToken)
    {
        await _container.CreateIfNotExistsAsync(cancellationToken: cancelationToken);
    }

    public async Task PutAsync(Guid id, string data, CancellationToken cancelationToken = default)
    {
        await EnsureContainerAsync(cancelationToken);

        var blob = _container.GetBlobClient(id.ToString());

        var bytes = Encoding.UTF8.GetBytes(data);
        using var stream = new MemoryStream(bytes);

        await blob.UploadAsync(stream, overwrite: true, cancellationToken: cancelationToken);
    }

    public async Task<string?> GetAsync(Guid id, CancellationToken cancelationToken = default)
    {
        var blob = _container.GetBlobClient(id.ToString());

        if (!await blob.ExistsAsync(cancelationToken))
        {
            return null;
        }

        var response = await blob.DownloadContentAsync(cancelationToken);

        return response.Value.Content.ToString();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancelationToken = default)
    {
        await _container.DeleteBlobIfExistsAsync(id.ToString(), cancellationToken: cancelationToken);
    }
}
