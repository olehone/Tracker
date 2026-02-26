using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchivingFunction.Domain.Entities;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardItemAttachmentConfiguration : IEntityTypeConfiguration<BoardItemAttachment>
{
    public void Configure(EntityTypeBuilder<BoardItemAttachment> builder)
    {
        builder.ToTable("BoardItemAttachments");

        builder.HasKey(attachment => attachment.Id);

        builder.HasOne(attachment => attachment.Item)
            .WithMany(item => item.Attachments)
            .HasForeignKey(attachment => attachment.BoardItemId);

        builder.Property(attachment => attachment.OriginalFileName)
            .IsRequired();

        builder.Property(attachment => attachment.ContentType)
            .IsRequired();

        builder.Property(attachment => attachment.SizeBytes)
            .IsRequired();

        builder.Property(attachment => attachment.StorageFileName)
            .IsRequired();

        builder.Property(attachment => attachment.StorageFolder)
            .IsRequired();

        builder.Property(attachment => attachment.UploadedAt)
            .IsRequired();

        builder.Property(attachment => attachment.IsDeleted)
            .IsRequired();
    }
}