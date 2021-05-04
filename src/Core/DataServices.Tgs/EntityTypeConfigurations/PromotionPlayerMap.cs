using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionPlayerMap : IEntityTypeConfiguration<PromotionPlayer>
    {
        void IEntityTypeConfiguration<PromotionPlayer>.Configure(EntityTypeBuilder<PromotionPlayer> builder)
        {
            builder.ToTable("promotion_players");

            builder.HasKey(c => c.Id)
                .HasName("pk_promotion_player_id");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.PromotionBonusId)
                .IsRequired();

            builder.Property(c => c.BonusAmount)
                .HasPrecision(18, 6);

            builder.Property(e => e.SentOn)
                .IsRequired(false);

            builder.HasOne(x => x.Player)
                .WithMany()
                .HasForeignKey(x => x.PlayerCode)
                .HasConstraintName("fk_promotion_bonus_player_code");

            builder.HasIndex(c => new {c.PlayerCode})
                .HasDatabaseName("ix_promotion_player_promotion_id");

            builder.HasIndex(c => new {c.PlayerCode, c.PromotionBonusId})
                .HasDatabaseName("ix_promotion_player_multiple_keys");
        }
    }
}