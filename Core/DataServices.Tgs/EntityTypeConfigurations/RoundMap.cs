using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class RoundMap : IEntityTypeConfiguration<Round>
    {
        void IEntityTypeConfiguration<Round>.Configure(EntityTypeBuilder<Round> builder)
        {
            builder.ToTable("rounds", Consts.BetSchema);

            builder.HasKey(bc => bc.Id)
                .HasName("pk_rounds");

            builder.Property(c => c.ExternalRoundId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameId)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.WalletId)
                .IsRequired();

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.ClosedOn);

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.Property(c => c.ExternalCreatedOn);

            builder.Property(c => c.ExternalClosedOn);

            builder.Property(c => c.BetAmount)
                .HasPrecision(18, 6);

            builder.Property(c => c.WinLoss)
                .HasPrecision(18, 6);

            builder.Property(c => c.TieBetAdjustment)
                .HasPrecision(18, 6);

            builder.Property(c => c.Commission)
                .HasPrecision(18, 6);

            builder.HasOne(c => c.GameProvider)
                .WithMany()
                .HasForeignKey(c => c.GameProviderId)
                .HasConstraintName("fk_rounds_game_providers_game_provider_id");

            builder.HasIndex(c => c.GameProviderId)
                .HasDatabaseName("ix_rounds_game_provider_id");

            builder.HasOne(c => c.GameProviderConfiguration)
                .WithMany()
                .HasForeignKey(c => c.GameProviderConfigurationId)
                .HasConstraintName("fk_rounds_game_providers_configurations_game_provider_configuration_id");
            
            builder.HasIndex(c => c.GameProviderConfigurationId)
                .HasDatabaseName("ix_rounds_game_provider_configuration_id");

            builder.HasOne(c => c.Game)
                .WithMany()
                .HasForeignKey(c => c.GameId)
                .HasConstraintName("fk_rounds_games_game_id");

            builder.HasIndex(c => c.GameId)
                .HasDatabaseName("ix_rounds_game_id");

            builder.HasOne(c => c.Wallet)
                .WithMany()
                .HasForeignKey(c => c.WalletId)
                .HasConstraintName("fk_rounds_wallets_wallet_id");

            builder.HasIndex(c => c.WalletId)
                .HasDatabaseName("ix_rounds_wallet_id");

            builder.HasOne(c => c.Player)
                .WithMany()
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_rounds_players_player_code");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_rounds_player_code");

            builder.HasOne(c => c.Brand)
                .WithMany()
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_rounds_brands_brand_id");

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_rounds_brand_id");

            builder.HasIndex(c => new {c.ExternalRoundId, c.PlayerCode, c.WalletId, c.GameProviderConfigurationId})
                .IsUnique()
                .HasDatabaseName("ux_rounds_external_round_id_composite");
        }
    }
}