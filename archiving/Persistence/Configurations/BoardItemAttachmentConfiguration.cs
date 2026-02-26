using ArchivingFunction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardItemAttachmentConfiguration : BaseEntityConfiguration<BoardItemAttachment>
{
    public override void Configure(EntityTypeBuilder<BoardItemAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("BoardItemAttachments");

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
