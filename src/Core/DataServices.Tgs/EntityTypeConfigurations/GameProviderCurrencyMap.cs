using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderCurrencyMap : IEntityTypeConfiguration<GameProviderCurrency>
    {
        void IEntityTypeConfiguration<GameProviderCurrency>.Configure(EntityTypeBuilder<GameProviderCurrency> builder)
        {
            builder.ToTable("game_provider_currencies");

            builder.HasKey(e => new {e.GameProviderId, e.CurrencyCode})
                .HasName("pk_game_provider_currencies");

            builder.Property(e => e.CurrencyCode)
                .HasMaxLength(16);

            builder.Property(e => e.GameProviderCurrencyCode)
                .HasMaxLength(16)
                .IsRequired();

            builder
                .HasOne(e => e.GameProvider)
                .WithMany(e => e.Currencies)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_game_provider_currencies_game_providers_game_provider_id");

            builder
                .HasOne(e => e.Currency)
                .WithMany(e => e.GameProviderCurrencies)
                .HasForeignKey(e => e.CurrencyCode)
                .HasConstraintName("fk_game_provider_currencies_currencies_currency_code");

            builder.HasIndex(e => e.CurrencyCode)
                .HasDatabaseName("ix_game_provider_currencies_currency_code");
        }
    }
}