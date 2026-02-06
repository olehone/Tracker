using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Services;

internal class AzureBlobAvatarStorageService : AzurePublicBlobStorageService, IAvatarStorageService
{
    public AzureBlobAvatarStorageService(BlobServiceClient blobServiceClient, IOptions<BlobOptions> options)
        : base(blobServiceClient, options.Value.AvatarContainerName)
    {
    }
}
