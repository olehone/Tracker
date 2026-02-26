using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class ItemCommentConfiguration : BaseEntityConfiguration<ItemComment>
{
    public override void Configure(EntityTypeBuilder<ItemComment> builder)
    {
        base.Configure(builder);

        builder.ToTable("ItemComments");

        builder.Property(c => c.Content).IsRequired();
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.UploadedAt).IsRequired();
        builder.Property(c => c.IsDeleted).IsRequired();

        builder.HasMany(c => c.Attachments)
            .WithOne()
            .HasForeignKey(a => a.ItemCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
