using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class WhitelistMap : IEntityTypeConfiguration<Whitelist>
    {
        void IEntityTypeConfiguration<Whitelist>.Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.ToTable("whitelists");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_whitelists");

            builder.Property(c => c.IpAddress)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(c => c.InternalNote)
                .HasMaxLength(256)
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

            builder.HasOne(c => c.Reseller)
                .WithMany()
                .HasForeignKey(c => c.ResellerId)
                .HasConstraintName("fk_whitelists_resellers_reseller_id");

            builder.HasIndex(c => c.ResellerId)
                .HasDatabaseName("ix_whitelists_reseller_id");

            builder.HasOne(c => c.Licensee)
                .WithMany()
                .HasForeignKey(c => c.LicenseeId)
                .HasConstraintName("fk_whitelists_licensees_licensee_id");

            builder.HasIndex(c => c.LicenseeId)
                .HasDatabaseName("ix_whitelists_licensee_id");

            builder.HasOne(c => c.Brand)
                .WithMany()
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_whitelists_brands_brand_id");

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_whitelists_brand_id");
        }
    }
}