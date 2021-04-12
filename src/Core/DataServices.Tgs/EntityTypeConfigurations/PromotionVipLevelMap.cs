using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionVipLevelMap : IEntityTypeConfiguration<PromotionVipLevel>
    {
        void IEntityTypeConfiguration<PromotionVipLevel>.Configure(EntityTypeBuilder<PromotionVipLevel> builder)
        {
            builder.ToTable("promotion_vip_levels");

            builder.HasKey(e => new {e.PromotionId, e.VipLevelId})
                .HasName("pk_promotion_vip_levels");

            builder.HasOne(p => p.Promotion)
                .WithMany(p => p.PromotionVipLevels)
                .HasForeignKey(p => p.PromotionId)
                .HasConstraintName("fk_promotion_vip_level_promotion_id");

            builder.HasOne(p => p.VipLevel)
                .WithMany()
                .HasForeignKey(p => p.VipLevelId)
                .HasConstraintName("fk_promotion_vip_level_vip_level_id");

            builder.HasIndex(c => c.Level)
                .HasDatabaseName("ix_promotion_vip_level_vip_level_id");
        }
    }
}