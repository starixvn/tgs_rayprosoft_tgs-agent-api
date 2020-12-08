using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandGameProviderMap : IEntityTypeConfiguration<BrandGameProvider>
    {
        void IEntityTypeConfiguration<BrandGameProvider>.Configure(EntityTypeBuilder<BrandGameProvider> builder)
        {
            builder.ToTable("BrandGameProviders", "dbo");

            builder.HasKey(bgp => new {bgp.BrandId, bgp.GameProviderId});

            builder.Property(bgp => bgp.Status)
                .IsRequired();

            builder.Property(c => c.DesktopQueryString)
                .HasColumnType(ColumnTypes.Nvarchar(512))
                .HasDefaultValue(string.Empty)
                .IsRequired();

            builder.Property(c => c.MobileQueryString)
                .HasColumnType(ColumnTypes.Nvarchar(512))
                .HasDefaultValue(string.Empty)
                .IsRequired();

            builder.Property(c => c.DefaultLanguageCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder
                .HasOne(bgp => bgp.GameProvider)
                .WithMany(gp => gp.Brands)
                .HasForeignKey(bgp => bgp.GameProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(bgp => bgp.Brand)
                .WithMany(b => b.GameProviders)
                .HasForeignKey(bgp => bgp.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bgp => bgp.GameProviderConfiguration)
                .WithMany(gpc => gpc.BrandGameProviders)
                .HasForeignKey(bgp => bgp.GameProviderConfigurationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}