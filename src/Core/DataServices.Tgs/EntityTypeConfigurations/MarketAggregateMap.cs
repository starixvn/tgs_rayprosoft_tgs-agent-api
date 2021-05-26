using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class MarketAggregateMap : IEntityTypeConfiguration<MarketAggregate>
    {
        void IEntityTypeConfiguration<MarketAggregate>.Configure(EntityTypeBuilder<MarketAggregate> builder)
        {
            builder.ToTable("market_aggregates", Consts.ReportSchema);

            builder.HasKey(b => b.Id)
                .HasName("pk_market_aggregates_id");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.BrandId)
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

            builder.Property(b => b.Turnover)
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

            builder.HasIndex(b => new {b.BrandId, b.ReportDate})
                .HasDatabaseName("ix_market_aggregates_brand_id_report_date");
        }
    }
}