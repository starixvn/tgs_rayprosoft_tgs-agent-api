using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGameProviderMap : IEntityTypeConfiguration<BrandGameProvider>
    {
        void IEntityTypeConfiguration<BrandGameProvider>.Configure(EntityTypeBuilder<BrandGameProvider> builder)
        {
            builder.ToTable("brand_game_providers");

            builder.HasKey(e => e.Id)
                .HasName("pk_brand_game_providers");

            builder.Property(e => e.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(e => e.DesktopQueryString)
                .HasMaxLength(512)
                .HasDefaultValue(string.Empty)
                .IsRequired();

            builder.Property(e => e.MobileQueryString)
                .HasMaxLength(512)
                .HasDefaultValue(string.Empty)
                .IsRequired();

            builder.Property(e => e.DefaultLanguageCode)
                .HasMaxLength(6)
                .IsRequired();

            builder
                .HasOne(e => e.GameProvider)
                .WithMany(e => e.Brands)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_brand_game_providers_game_providers_game_provider_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.GameProviderId)
                .HasDatabaseName("ix_brand_game_providers_game_provider_id");

            builder
                .HasOne(e => e.Brand)
                .WithMany(e => e.GameProviders)
                .HasForeignKey(e => e.BrandId)
                .HasConstraintName("fk_brand_game_providers_brands_brand_id");

            builder.HasIndex(e => new {e.BrandId, e.GameProviderId})
                .IsUnique()
                .HasDatabaseName("ux_brand_game_providers_brand_id_game_provider_id")
                .IncludeProperties(e => new
                {
                    e.GameProviderConfigurationId,
                    e.Status,
                    e.DefaultLanguageCode,
                    e.DesktopQueryString,
                    e.MobileQueryString
                });

            builder.HasOne(e => e.GameProviderConfiguration)
                .WithMany(e => e.BrandGameProviders)
                .HasForeignKey(e => e.GameProviderConfigurationId)
                .HasConstraintName(
                    "fk_brand_game_providers_game_provider_configurations_game_provider_configuration_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.GameProviderConfigurationId)
                .HasDatabaseName("ix_brand_game_providers_game_provider_configuration_id");

            builder.HasOne(e => e.DefaultLanguage)
                .WithMany(e => e.BrandGameProviders)
                .HasForeignKey(e => e.DefaultLanguageCode)
                .HasConstraintName("fk_brand_game_providers_languages_default_language_code");

            builder.HasIndex(e => e.DefaultLanguageCode)
                .HasDatabaseName("ix_brand_game_providers_default_language_code");
        }
    }
}