using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandMap : BaseConfiguration, IEntityTypeConfiguration<Brand>
    {
        void IEntityTypeConfiguration<Brand>.Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("brands");

            ConfigureBaseProperties<Brand, Guid>(builder, "pk_brands");

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(256);

            builder.Property(c => c.OperationalEmail)
                .HasMaxLength(128);

            builder.Property(c => c.ClientId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(c => c.SecretKey)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.ApiKey)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.TimeZoneId)
                .HasMaxLength(256);

            builder.HasOne(c => c.Licensee)
                .WithMany(c => c.Brands)
                .HasForeignKey(c => c.LicenseeId)
                .HasConstraintName("fk_brands_licensees_licensee_id");

            builder.HasIndex(c => c.LicenseeId)
                .HasDatabaseName("ix_brands_licensee_id");
        }
    }
}