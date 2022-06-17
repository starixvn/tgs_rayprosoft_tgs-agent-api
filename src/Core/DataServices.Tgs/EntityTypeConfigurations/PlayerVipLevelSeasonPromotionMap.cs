using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerVipLevelSeasonPromotionMap : IEntityTypeConfiguration<PlayerVipLevelSeasonPromotion>
{
    void IEntityTypeConfiguration<PlayerVipLevelSeasonPromotion>.Configure(EntityTypeBuilder<PlayerVipLevelSeasonPromotion> builder)
    {
        builder.ToTable("player_vip_level_season_promotions");

        builder.HasKey(b => b.Id)
            .HasName("pk_player_vip_level_season_promotions");
        builder.Property(b => b.Id)
        .IsRequired();
        builder.Property(b => b.VipId)
            .IsRequired();
        builder.Property(b => b.PlayerVipLevelSnapshotId)
            .IsRequired();
        builder.Property(b => b.PlayerCode)
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(b => b.SeasonOn)
            .IsRequired();
        builder.Property(b => b.FromOn)
            .IsRequired();
        builder.Property(b => b.ToOn)
            .IsRequired();
        builder.Property(b => b.Amount)
            .HasPrecision(18, 6)
            .IsRequired();
        builder.Property(b => b.CreatedOn)
            .IsRequired();
        builder.HasIndex(b => b.VipId)
            .HasDatabaseName("ix_player_vip_level_season_promotions_vip_id");
        builder.HasIndex(b => b.PlayerVipLevelSnapshotId)
            .HasDatabaseName("ix_player_vip_level_season_promotions_player_vip_level_snapshot_id");
    }
}
