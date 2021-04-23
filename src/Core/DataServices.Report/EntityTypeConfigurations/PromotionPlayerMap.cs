using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Report.Models;

namespace TGS.Core.DataServices.Report.EntityTypeConfigurations
{
    internal class PromotionPlayerMap : IEntityTypeConfiguration<PromotionPlayer>
    {
        void IEntityTypeConfiguration<PromotionPlayer>.Configure(EntityTypeBuilder<PromotionPlayer> builder)
        {
            builder.ToTable("promotion_player");

            builder.HasKey(c => c.Id)
                .HasName("pk_promotion_player_id");

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.PromotionBonusId)
                .IsRequired();

            builder.Property(c => c.BonusAmount)
                .HasPrecision(18, 6);

            builder.Property(e => e.SentOn)
                .IsRequired();

            builder.HasIndex(c => new {c.PlayerCode})
                .HasDatabaseName("ix_promotion_player_promotion_id");

            builder.HasIndex(c => new {c.PlayerCode, c.PromotionBonusId})
                .HasDatabaseName("ix_promotion_player_multiple_keys");
        }
    }
}