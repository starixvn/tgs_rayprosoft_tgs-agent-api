using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

public class DistributeRegisterEventMap : IEntityTypeConfiguration<DistributeRegisterEvent>
{
    public void Configure(EntityTypeBuilder<DistributeRegisterEvent> builder)
    {
        builder.ToTable("distribute_register_events");

        builder.Property(rs => rs.Id)
            .IsRequired();

        builder.HasKey(rs => rs.Id)
            .HasName("pk_distribute_register_events");

        builder.Property(rs => rs.BrandId)
            .IsRequired();

        builder.Property(rs => rs.PlayerCode)
            .IsRequired();

        builder.Property(rs => rs.CreatedOn)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("Now()");

        builder.Property(rs => rs.PromotionId)
            .IsRequired();

        builder.Property(rs => rs.Bonus)
            .HasDefaultValue(0M)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(rs => rs.BrandId)
            .HasConstraintName("fk_distribute_register_events_brands_brand_id")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Player>()
            .WithMany()
            .HasForeignKey(rs => rs.PlayerCode)
            .HasConstraintName("fk_distribute_register_events_players_brand_id")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Promotion>()
            .WithMany()
            .HasForeignKey(rs => rs.PromotionId)
            .HasConstraintName("fk_distribute_register_events_promotions_promotion_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
