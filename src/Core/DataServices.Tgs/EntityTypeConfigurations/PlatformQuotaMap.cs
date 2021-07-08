using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlatformQuotaMap : BaseConfiguration, IEntityTypeConfiguration<PlatformCredit>
    {
        void IEntityTypeConfiguration<PlatformCredit>.Configure(EntityTypeBuilder<PlatformCredit> builder)
        {
            builder.ToTable("brand_credits");

            builder.HasKey(p => p.Id)
                .HasName("pk_brand_credits");

            builder.Property(e => e.BrandId)
                .IsRequired();

            builder.Property(c => c.Credit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.Balance)
                .HasPrecision(18, 6)
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

            builder.HasOne(e => e.Brand)
                .WithOne()
                .HasForeignKey<PlatformCredit>(e => e.BrandId)
                .HasConstraintName("fk_brand_credits_brands_brand_id")
                .IsRequired();

            builder.HasIndex(e => e.BrandId)
                .HasDatabaseName("ix_brand_credits_brand_id");
        }
    }
}