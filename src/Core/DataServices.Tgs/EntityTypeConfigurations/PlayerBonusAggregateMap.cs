using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerBonusAggregateMap : IEntityTypeConfiguration<PlayerBonusAggregate>
{
    void IEntityTypeConfiguration<PlayerBonusAggregate>.Configure(EntityTypeBuilder<PlayerBonusAggregate> builder)
    {
        builder.ToTable("player_bonus_aggregates", Consts.ReportSchema);

        builder.HasKey(b => new { b.PlayerCode,b.Hour,b.Minute,b.ReportDate })
            .HasName("pk_player_bonus_aggregates_id");

        builder.Property(c => c.PlayerCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(b => b.RebateAmount)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(b => b.PromotionAmount)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(b => b.UpgradeAmount)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(b => b.SeasonAmount)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(b => b.BirthdayAmount)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(b => b.ReportDate)
            .IsRequired();

        builder.Property(b => b.CreatedOn)
            .IsRequired();

        builder.HasIndex(b => new
            {
                b.ReportDate,
                b.BrandId,
                b.AgentId,
                b.PlayerCode
            })
            .HasDatabaseName("ix_player_bonus_aggregates_multiple_keys");
    }
}