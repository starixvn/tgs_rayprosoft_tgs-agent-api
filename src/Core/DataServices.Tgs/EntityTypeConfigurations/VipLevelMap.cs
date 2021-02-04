using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class VipLevelMap : BaseConfiguration, IEntityTypeConfiguration<VipLevel>
    {
        void IEntityTypeConfiguration<VipLevel>.Configure(EntityTypeBuilder<VipLevel> builder)
        {
            builder.ToTable("vip_levels");

            ConfigureBaseProperties<VipLevel, Guid>(builder, "pk_vip_levels");

            builder.HasAlternateKey(c => new {c.BrandId, c.Code})
                .HasName("ak_vip_levels_brand_id_code");

            builder.Property(c => c.Code)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder
                .HasOne(c => c.Brand)
                .WithMany(c => c.VipLevels)
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_vip_levels_brands_brand_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_vip_levels_brand_id");
        }
    }
}