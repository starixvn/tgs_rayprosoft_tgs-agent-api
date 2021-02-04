using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderLanguageMap : IEntityTypeConfiguration<GameProviderLanguage>
    {
        void IEntityTypeConfiguration<GameProviderLanguage>.Configure(EntityTypeBuilder<GameProviderLanguage> builder)
        {
            builder.ToTable("game_provider_languages");

            builder.HasKey(e => new {e.GameProviderId, e.CultureCode})
                .HasName("pk_game_provider_languages");

            builder.Property(e => e.CultureCode)
                .HasMaxLength(6);

            builder.Property(e => e.GameProviderCultureCode)
                .HasMaxLength(6)
                .IsRequired();

            builder
                .HasOne(e => e.GameProvider)
                .WithMany(e => e.Languages)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_game_provider_languages_game_providers_game_provider_id");

            builder
                .HasOne(e => e.Language)
                .WithMany(e => e.GameProviderLanguages)
                .HasForeignKey(e => e.CultureCode)
                .HasConstraintName("fk_game_provider_languages_languages_culture_code");

            builder.HasIndex(e => e.CultureCode)
                .HasDatabaseName("ix_game_provider_languages_culture_code");
        }
    }
}