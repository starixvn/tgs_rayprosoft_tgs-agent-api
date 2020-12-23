using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandMap : BaseConfiguration, IEntityTypeConfiguration<Brand>
    {
        void IEntityTypeConfiguration<Brand>.Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands", "dbo");
            BaseConfigureProperties<Brand, Guid>(builder);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.OperationalEmail)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(c => c.ClientId)
                .HasColumnType(ColumnTypes.Varchar(128))
                .IsRequired();

            builder.Property(c => c.SecretKey)
                .HasColumnType(ColumnTypes.Varchar(32))
                .IsRequired();

            builder.Property(c => c.ApiKey)
                .HasColumnType(ColumnTypes.Varchar(64))
                .IsRequired();

            builder.Property(c => c.TimeZoneId)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.HasMany(c => c.GameProviders)
                .WithOne(c => c.Brand)
                .HasForeignKey(c => c.GameProviderId);

            builder.HasMany(c => c.Players)
                .WithOne(c => c.Brand)
                .HasForeignKey(c => c.BrandId);

            builder.HasOne(p => p.Licensee)
                .WithMany(p => p.Brands)
                .HasForeignKey(p => p.LicenseeId);

            builder.HasMany(c => c.Announcements)
                .WithOne(c => c.Brand)
                .HasForeignKey(c => c.BrandId);
        }
    }
}