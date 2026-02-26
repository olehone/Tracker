using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class CommentAttachmentConfiguration : BaseEntityConfiguration<CommentAttachment>
{
    public override void Configure(EntityTypeBuilder<CommentAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("CommentAttachments");

        builder.Property(a => a.OriginalFileName).IsRequired();
        builder.Property(a => a.ContentType).IsRequired();
        builder.Property(a => a.SizeBytes).IsRequired();
        builder.Property(a => a.StorageFileName).IsRequired();
        builder.Property(a => a.StorageFolder).IsRequired();
        builder.Property(a => a.UploadedAt).IsRequired();
        builder.Property(a => a.IsDeleted).IsRequired();
        builder.Property(a => a.UserId).IsRequired();
    }
}