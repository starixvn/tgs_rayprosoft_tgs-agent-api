using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionParticipantMap : IEntityTypeConfiguration<PromotionParticipant>
    {
        void IEntityTypeConfiguration<PromotionParticipant>.Configure(EntityTypeBuilder<PromotionParticipant> builder)
        {
            builder.ToTable("promotion_participants");

            builder.HasKey(b => new {b.PromotionId, b.PlayerCode})
                .HasName("pk_promotion_participants");

            builder.HasOne(p => p.Promotion)
                .WithMany(p => p.PromotionParticipants)
                .HasForeignKey(p => p.PromotionId)
                .HasConstraintName("fk_promotion_participant_promotion_id");

            builder.HasOne(p => p.Player)
                .WithMany()
                .HasForeignKey(p => p.PlayerCode)
                .HasConstraintName("fk_promotion_participant_player_code");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_promotion_participant_player_code");
        }
    }
}