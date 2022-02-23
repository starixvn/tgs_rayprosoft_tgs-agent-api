using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderAggregateMap : IEntityTypeConfiguration<GameProviderAggregate>
    {
        void IEntityTypeConfiguration<GameProviderAggregate>.Configure(
            EntityTypeBuilder<GameProviderAggregate> builder)
        {
            builder.ToTable("game_provider_aggregates", Consts.ReportSchema);

            builder.HasKey(c => c.Id)
                .HasName("pk_game_provider_aggregate_id");

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.GameProviderId)
                .IsRequired();

            builder.Property(c => c.CurrencyCode)
                .IsRequired();

            builder.Property(c => c.BetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.ValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.Turnover)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.WinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.ExternalClosedOn)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => new
                {
                    c.ExternalClosedOn,
                    c.GameProviderId,
                    c.BrandId
                })
                .HasDatabaseName("ix_game_provider_aggregate_multiple_keys");
        }
    }
}