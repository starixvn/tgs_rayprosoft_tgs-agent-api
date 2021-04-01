using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report.Models;

namespace TGS.Core.DataServices.Report.EntityTypeConfigurations
{
    internal class PlayerGameTypeAggregateMap : IEntityTypeConfiguration<PlayerGameTypeAggregate>
    {
        void IEntityTypeConfiguration<PlayerGameTypeAggregate>.Configure(
            EntityTypeBuilder<PlayerGameTypeAggregate> builder)
        {
            builder.ToTable("player_game_type_rebate_aggregates");

            builder.HasKey(b => b.Id)
                .HasName("pk_player_game_type_rebate_aggregates_id");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.Turnover)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.WinLoss)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipLiveDealerRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipSlotRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipCardGameRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipSportsRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipVirtualSportsRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipLotteryRebatePercentage)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VipRebateAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ReportDate)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.HasIndex(b => new {b.BrandId, b.ReportDate})
                .HasDatabaseName("ix_player_game_type_rebate_aggregates_brand_id_report_date");

            builder.HasIndex(b => new {b.BrandId, b.ReportDate, b.PlayerCode})
                .HasDatabaseName("ix_player_game_type_rebate_aggregates_brand_id_report_date_player_code");

            builder.HasIndex(b => new {b.BrandId, b.ReportDate, b.AgentId})
                .HasDatabaseName("ix_player_game_type_rebate_aggregates_brand_id_report_date_agent_id");
        }
    }
}