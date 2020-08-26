using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandGameProviderConfigurationMap : IEntityTypeConfiguration<BrandGameProviderConfiguration>
    {
        void IEntityTypeConfiguration<BrandGameProviderConfiguration>.Configure(EntityTypeBuilder<BrandGameProviderConfiguration> builder)
        {
            builder.ToTable("BrandGameProviderConfigurations", "dbo");
            builder.HasKey(c => c.Id);
            builder.HasAlternateKey(c => new {c.BrandId, c.GameProviderId})
                .HasName("UK_BrandGameProviderConfigurations_BrandId_GameProviderId");

            builder.Property(c => c.BrandLogoUrl)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.DesktopQueryString)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.MobileQueryString)
                .HasColumnType("nvarchar(512)");

            builder.Property(c => c.DefaultLanguageCode)
                .HasColumnType("nvarchar(6)");

            builder.HasOne(c => c.Brand)
                .WithMany(c => c.BrandGameProviderConfigurations)
                .HasForeignKey(c => c.BrandId);

            builder.HasOne(c => c.GameProvider)
                .WithMany(c => c.BrandGameProviderConfigurations)
                .HasForeignKey(c => c.GameProviderId);
        }
    }
}