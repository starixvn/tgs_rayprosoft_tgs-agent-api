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

            builder.Property(c => c.AuditLogId)
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

            builder.Property(c => c.SettleAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.Commission)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.ReportDate)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => c.AuditLogId)
                .HasDatabaseName("ix_game_provider_aggregate_audit_log_id");

            builder.HasIndex(c => new
                {
                    c.ReportDate,
                    c.GameProviderId
                })
                .HasDatabaseName("ix_game_provider_aggregate_multiple_keys");
        }
    }
}