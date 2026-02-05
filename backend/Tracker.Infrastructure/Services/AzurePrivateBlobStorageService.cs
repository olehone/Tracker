using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.Services;

// Separate container/folder name in case that same entity would have different containers
internal class AzurePrivateBlobStorageService(BlobServiceClient blobServiceClient, TimeSpan expiration) : IAzurePrivateBlobStorageService
{
    public async Task<string> GetUrl(string folderName, string fileName, CancellationToken cancellationToken = default)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        var blob = container.GetBlobClient(fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = folderName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiration),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blob.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string folderName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobName = Guid.NewGuid().ToString();
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return blobName;
    }

    public async Task DeleteAsync(string folderName, string fileName, CancellationToken cancellationToken = default)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        await container.GetBlobClient(fileName)
            .DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
