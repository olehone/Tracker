using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Services;

internal class AzureBlobAttachmentStorageService : AzurePrivateBlobStorageService, IAttachmentStorageService
{
    public AzureBlobAttachmentStorageService(BlobServiceClient blobServiceClient, IOptions<BlobOptions> options)
        : base(blobServiceClient, options.Value.ItemAttachmentExpiration)
    {
    }
}
