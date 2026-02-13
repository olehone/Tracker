using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class CommentAttachmentConfiguration : IEntityTypeConfiguration<CommentAttachment>
{
    public void Configure(EntityTypeBuilder<CommentAttachment> builder)
    {
        builder.ToTable("CommentAttachments");

        builder.HasKey(attachment => attachment.Id);

        builder.HasOne(attachment => attachment.Comment)
            .WithMany(comment => comment.Attachments)
            .HasForeignKey(attachment => attachment.ItemCommentId);

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