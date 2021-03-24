using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Report.Models;

namespace TGS.Core.DataServices.Report.EntityTypeConfigurations
{
    internal class PlayerAggregateMap : IEntityTypeConfiguration<PlayerAggregate>
    {
        void IEntityTypeConfiguration<PlayerAggregate>.Configure(EntityTypeBuilder<PlayerAggregate> builder)
        {
            builder.ToTable("player_aggregates");

            builder.HasKey(b => b.Id)
                .HasName("pk_player_aggregates_id");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(b => b.RegisteredCount)
                .IsRequired();

            builder.Property(b => b.FirstFundInCount)
                .IsRequired();

            builder.Property(b => b.DepositAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.WithdrawalAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.AdjustmentAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.BonusAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.BetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.WinLossAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.NetProfit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ReportDate)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.HasIndex(b => new {b.BrandId, b.ReportDate})
                .HasDatabaseName("ix_player_aggregates_brand_id_report_date");

            builder.HasIndex(b => new {b.BrandId, b.ReportDate, b.PlayerCode})
                .HasDatabaseName("ix_player_aggregates_brand_id_report_date_player_code");

            builder.HasIndex(b => new {b.BrandId, b.ReportDate, b.AgentId})
                .HasDatabaseName("ix_player_aggregates_brand_id_report_date_agent_id");
        }
    }
}