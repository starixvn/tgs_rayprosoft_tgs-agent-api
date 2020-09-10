using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeGameProviderConfigurationMap : IEntityTypeConfiguration<LicenseeGameProviderConfiguration>
    {
        void IEntityTypeConfiguration<LicenseeGameProviderConfiguration>.Configure(EntityTypeBuilder<LicenseeGameProviderConfiguration> builder)
        {
            builder.ToTable("LicenseeGameProviderConfigurations", "dbo");
            builder.HasKey(c => c.Id);
            builder.HasAlternateKey(c => new {c.LicenseeId, c.GameProviderId})
                .HasName("UK_LicenseeGameProviderConfigurations_LicenseeId_GameProviderId");

            builder.Property(c => c.LicenseeLogoUrl)
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