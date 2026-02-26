using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ArchivingFunction.Infrastructure;

internal class AzureBlobStorageService(BlobServiceClient blobServiceClient, TimeSpan expiration)
{
}
