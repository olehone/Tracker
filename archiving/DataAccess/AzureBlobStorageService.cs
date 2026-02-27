using Azure.Storage.Blobs;

namespace DataAccess;

internal class AzureBlobStorageService(BlobServiceClient blobServiceClient, TimeSpan expiration)
{
}
