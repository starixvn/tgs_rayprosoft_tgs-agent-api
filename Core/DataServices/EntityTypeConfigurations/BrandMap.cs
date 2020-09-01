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
            BaseConfigureProperties(builder);

            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Brands)
                .HasForeignKey(p => p.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.TimeZoneId)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.LoginUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.CashierUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.HelpUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.TermsUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileLoginUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileCashierUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileHelpUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileTermsUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.HasMany(c => c.BrandGameProviderConfigurations)
                .WithOne(c => c.Brand)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
