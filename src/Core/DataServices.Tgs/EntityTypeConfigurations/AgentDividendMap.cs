using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AgentDividendMap : IEntityTypeConfiguration<AgentDividend>
    {
        void IEntityTypeConfiguration<AgentDividend>.Configure(EntityTypeBuilder<AgentDividend> builder)
        {
            builder.ToTable("agent_dividends");

            builder.HasKey(b => b.Id)
                .HasName("pk_agent_dividends_id");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

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

            builder.Property(b => b.LiveDealerWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SlotWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.FishingWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ArcadeWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.TableGameWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.LotteryWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.CardGameWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SportsWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ESportsWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.VirtualSportsWinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.AgentDividendPercentage)
                .HasPrecision(9, 6)
                .IsRequired();

            builder.Property(b => b.From)
                .IsRequired();

            builder.Property(b => b.To)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.Property(e => e.SentOn)
                .IsRequired(false);

            builder.HasIndex(b => b.BrandId)
                .HasDatabaseName("ix_agent_dividends_brand_id");

            builder.HasIndex(b => new {b.BrandId, b.CreatedOn})
                .HasDatabaseName("ix_agent_dividends_brand_id_created_on");

            builder.HasIndex(b => new {b.BrandId, b.CreatedOn, b.AgentId})
                .HasDatabaseName("ix_agent_dividends_brand_id_created_on_agent_id");
        }
    }
}