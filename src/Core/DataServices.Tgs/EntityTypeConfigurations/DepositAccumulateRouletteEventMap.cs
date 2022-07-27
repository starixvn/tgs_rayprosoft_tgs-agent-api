using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class DepositAccumulateRouletteEventMap : IEntityTypeConfiguration<DepositAccumulateRouletteEvent>
{
    public void Configure(EntityTypeBuilder<DepositAccumulateRouletteEvent> builder)
    {
        builder.ToTable("deposit_accumulate_roulette_events");

        builder.HasKey(e => e.Id)
            .HasName("pk_deposit_accumulate_roulette_events");

        builder.Property(e => e.PlayerCode)
            .IsRequired();

        builder.Property(e => e.BrandId)
            .IsRequired();

        builder.Property(e => e.DistributeTime)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(e => e.CreatedOn)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("Now()");

        builder.Property(e => e.PromotionId)
            .IsRequired();

        builder.Property(e => e.RouletteCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.SettingDepositAmount)
            .IsRequired()
            .HasDefaultValue(0M)
            .HasPrecision(18, 6);

        builder.Property(e => e.TotalDepositAmount)
            .IsRequired()
            .HasDefaultValue(0M)
            .HasPrecision(18, 6);

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(e => e.BrandId)
            .HasConstraintName("fk_deposit_accumulate_roulette_events_brands_brand_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Player>()
            .WithMany()
            .HasForeignKey(e => e.PlayerCode)
            .HasConstraintName("fk_deposit_accumulate_roulette_events_players_player_code")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Promotion>()
            .WithMany()
            .HasForeignKey(e => e.PromotionId)
            .HasConstraintName("fk_deposit_accumulate_roulette_events_promotions_promotion_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
