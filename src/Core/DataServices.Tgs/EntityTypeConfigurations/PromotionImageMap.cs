using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionImageMap : IEntityTypeConfiguration<PromotionImage>
    {
        void IEntityTypeConfiguration<PromotionImage>.Configure(EntityTypeBuilder<PromotionImage> builder)
        {
            builder.ToTable("promotion_images");

            builder.HasKey(b => b.Id)
                .HasName("pk_promotion_images");

            builder.Property(c => c.Url)
                .HasMaxLength(2048)
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .IsRequired();

            builder.HasOne(p => p.Promotion)
                .WithMany(p => p.PromotionImages)
                .HasForeignKey(p => p.PromotionId)
                .HasConstraintName("fk_promotion_image_promotion_id");

            builder.HasIndex(c => c.PromotionId)
                .HasDatabaseName("ix_promotion_image_promotion_id");
        }
    }
}