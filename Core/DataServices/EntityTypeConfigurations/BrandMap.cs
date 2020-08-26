using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)");

            builder.Property(c => c.TimeZoneId)
                .HasColumnType("nvarchar(256)");

            builder.Property(c => c.LoginUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.CashierUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.HelpUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.TermsUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.MobileLoginUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.MobileCashierUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.MobileHelpUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.MobileTermsUrl)
                .HasColumnType("nvarchar(512)");

            builder.HasMany(c => c.BrandGameProviderConfigurations)
                .WithOne(c => c.Brand)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
