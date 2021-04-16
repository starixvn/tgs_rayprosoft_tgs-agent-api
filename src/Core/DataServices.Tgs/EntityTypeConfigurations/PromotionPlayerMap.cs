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

            builder.HasKey(b => new {b.PromotionId, b.PlayerCode})
                .HasName("pk_promotion_players");

            builder.HasOne(p => p.Promotion)
                .WithMany(p => p.PromotionPlayers)
                .HasForeignKey(p => p.PromotionId)
                .HasConstraintName("fk_promotion_player_promotion_id");

            builder.HasOne(p => p.Player)
                .WithMany()
                .HasForeignKey(p => p.PlayerCode)
                .HasConstraintName("fk_promotion_player_player_code");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_promotion_player_player_code");
        }
    }
}