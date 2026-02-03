using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Services;

internal class AvatarStorageService : PublicStorageService, IAvatarStorageService
{
    public AvatarStorageService(BlobServiceClient blobServiceClient, IOptions<BlobOptions> options)
        : base(blobServiceClient, options.Value.AvatarContainerName)
    {
    }
}
