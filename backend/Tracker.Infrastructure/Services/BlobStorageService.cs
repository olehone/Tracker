using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.Services;

internal class BlobStorageService(BlobServiceClient blobServiceClient) : IStorageService
{
    private const string ContainerName = "avatars";

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

        var fileId = Guid.NewGuid();
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.UploadAsync(stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken
            );

        return fileId;
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);

        return new FileResponse(response.Value.Content.ToStream(), response.Value.Details.ContentType.ToString());
    }

    public async Task<Result<string>> GetUrlAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var properties = await containerClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileId.ToString());

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
