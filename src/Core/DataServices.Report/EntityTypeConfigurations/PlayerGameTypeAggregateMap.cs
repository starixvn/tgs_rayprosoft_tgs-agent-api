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

            builder.Property(b => b.LiveDealerAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SlotAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.CardGameAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SportsAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.VirtualSportsAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.LiveDealerValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SlotValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.CardGameValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.SportsValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.VirtualSportsValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.LiveDealerRebate)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.SlotRebate)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.CardGameRebate)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.SportsRebate)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.VirtualSportsRebate)
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