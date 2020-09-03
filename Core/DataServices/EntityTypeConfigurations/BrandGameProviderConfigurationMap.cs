using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
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
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.DesktopQueryString)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileQueryString)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.DefaultLanguageCode)
                .HasColumnType(ColumnTypes.Nvarchar(6));
        }
    }
}