using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Tracker.Infrastructure.Services;

internal class PublicStorageService
{
    private readonly BlobContainerClient _containerClient;

    public PublicStorageService(BlobServiceClient blobServiceClient, string containerName)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public string GetPublicUrl(Guid resourceId)
    {
        var blobClient = _containerClient.GetBlobClient(resourceId.ToString());
        return blobClient.Uri.ToString();
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string contentType,
        Guid resourceId,
        CancellationToken cancellationToken)
    {
        var blob = _containerClient.GetBlobClient(resourceId.ToString());

        await blob.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return GetPublicUrl(resourceId);
    }

    public async Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        await _containerClient.GetBlobClient(resourceId.ToString())
            .DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
