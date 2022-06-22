using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

public class RouletteSignInEventMap : IEntityTypeConfiguration<RouletteSignInEvent>
{
    public void Configure(EntityTypeBuilder<RouletteSignInEvent> builder)
    {
        builder.ToTable("roulette_sign_in_events");

        builder.Property(rs => rs.Id)
            .IsRequired();

        builder.HasKey(rs => rs.Id)
            .HasName("pk_roulette_sign_in_events");

        builder.Property(rs => rs.BrandId)
            .IsRequired();
        builder.Property(rs => rs.PlayerCode)
            .IsRequired();
        builder.Property(rs => rs.Date)
            .IsRequired()
            .HasColumnType("date");
        builder.Property(rs => rs.CreatedOn)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("Now()");

        builder.Property(rs => rs.PromotionId)
            .IsRequired();

        builder.Property(rs => rs.ItemIndex)
            .IsRequired();

        builder.Property(rs => rs.Bonus)
            .HasDefaultValue(0M)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(rs => rs.BrandId)
            .HasConstraintName("fk_roulette_sign_in_events_brands_brand_id")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Player>()
            .WithMany()
            .HasForeignKey(rs => rs.PlayerCode)
            .HasConstraintName("fk_roulette_sign_in_events_players_brand_id")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Promotion>()
            .WithMany()
            .HasForeignKey(rs => rs.PromotionId)
            .HasConstraintName("fk_roulette_sign_in_events_promotions_promotion_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
