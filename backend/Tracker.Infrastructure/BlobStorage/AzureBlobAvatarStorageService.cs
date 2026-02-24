using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.Services;

internal class AzureBlobAvatarStorageService : AzureBlobStorageService, IAvatarStorageService
{
    private readonly BlobOptions _options;

    public AzureBlobAvatarStorageService(BlobServiceClient blobServiceClient, IOptions<BlobOptions> options)
        : base(blobServiceClient, options.Value.AvatarSasExpiration)
    {
        _options = options.Value;
    }

    public Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default)
        => DeleteAsync(_options.AvatarContainerName, resourceId.ToString(), cancellationToken);

    public Task<Result<string>> GetUrlAsync(Guid resourceId, CancellationToken cancellationToken = default)
        => GetUrlAsync(_options.AvatarContainerName, resourceId.ToString(), string.Empty, true, cancellationToken);

    public Task<string> UploadAsync(Stream stream, string contentType, Guid resourceId, CancellationToken cancellationToken)
        => UploadAsync(stream, _options.AvatarContainerName, contentType, cancellationToken, resourceId.ToString());
}
