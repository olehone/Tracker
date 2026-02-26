using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.Services;

// Separate container/folder name in case that same entity would have different containers
internal class AzureBlobStorageService(BlobServiceClient blobServiceClient, TimeSpan expiration)
{
    public async Task<Result<string>> GetUrlAsync(string folderName, string fileName, string originalName,
        bool isInline, CancellationToken cancellationToken = default)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        var blob = container.GetBlobClient(fileName);
        var isExist = await blob.ExistsAsync(cancellationToken);
        if (!isExist)
        {
            return Error.NotFound("File");
        }

        var dispositionType = isInline ? "inline" : "attachment";

        var encodedFileName = Uri.EscapeDataString(originalName);
        var contentDisposition = $"{dispositionType}; filename*=UTF-8''{encodedFileName}";

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = folderName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiration),
            ContentDisposition = contentDisposition.ToString()
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blob.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task<Result<Stream>> GetStreamAsync(string folderName,
        string fileName, CancellationToken cancellationToken = default)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        var blob = container.GetBlobClient(fileName);
        var isExist = await blob.ExistsAsync(cancellationToken);
        if (!isExist)
        {
            return Error.NotFound("File");
        }

        var stream = await blob.OpenReadAsync(cancellationToken: cancellationToken);
        return stream;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string folderName,
        string contentType,
        CancellationToken cancellationToken = default,
        string? fileName = null)
    {
        var container = blobServiceClient.GetBlobContainerClient(folderName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobName = fileName ?? Guid.NewGuid().ToString();
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
