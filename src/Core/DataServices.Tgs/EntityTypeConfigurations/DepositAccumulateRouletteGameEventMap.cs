using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class DepositAccumulateRouletteGameEventMap : IEntityTypeConfiguration<DepositAccumulateRouletteGameEvent>
{
    public void Configure(EntityTypeBuilder<DepositAccumulateRouletteGameEvent> builder)
    {
        builder.ToTable("deposit_accumulate_roulette_game_events");

        builder.Property(e => e.Id)
            .IsRequired();

        builder.HasKey(e => e.Id)
            .HasName("pk_deposit_accumulate_roulette_game_events");

        builder.Property(e => e.BrandId)
            .IsRequired();

        builder.Property(e => e.PlayerCode)
            .IsRequired();

        builder.Property(e => e.CreatedOn)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("Now()");

        builder.Property(e => e.PromotionId)
            .IsRequired();

        builder.Property(e => e.ItemIndex)
            .IsRequired();

        builder.Property(e => e.Bonus)
            .HasDefaultValue(0M)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.DepositAccumulateRouletteEventId)
            .IsRequired();

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(e => e.BrandId)
            .HasConstraintName("fk_game_events_brands_brand_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Player>()
            .WithMany()
            .HasForeignKey(e => e.PlayerCode)
            .HasConstraintName("fk_game_events_players_brand_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Promotion>()
            .WithMany()
            .HasForeignKey(e => e.PromotionId)
            .HasConstraintName("fk_game_events_promotions_promotion_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<DepositAccumulateRouletteEvent>()
            .WithMany()
            .HasForeignKey(e => e.Id)
            .HasConstraintName("fk_game_events_deposit_accumulate_roulette_events_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
