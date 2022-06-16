using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerVipLevelUpgradePromotionMap : IEntityTypeConfiguration<PlayerVipLevelUpgradePromotion>
{
    void IEntityTypeConfiguration<PlayerVipLevelUpgradePromotion>.Configure(EntityTypeBuilder<PlayerVipLevelUpgradePromotion> builder)
    {
        builder.ToTable("player_vip_level_upgrade_promotions");

        builder.HasKey(b => b.Id)
            .HasName("pk_player_vip_level_upgrade_promotions");
        builder.Property(b => b.Id)
        .IsRequired();
        builder.Property(b => b.VipId)
            .IsRequired();
        builder.Property(b => b.PlayerVipLevelSnapshotId)
            .IsRequired();
        builder.Property(b => b.PlayerCode)
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(b => b.Amount)
            .HasPrecision(18, 6)
            .IsRequired();
        builder.Property(b => b.CreatedOn)
            .IsRequired();
        builder.HasIndex(b => b.VipId)
            .HasDatabaseName("ix_player_vip_level_upgrade_promotions_vip_id");
        builder.HasIndex(b => b.PlayerVipLevelSnapshotId)
            .HasDatabaseName("ix_player_vip_level_upgrade_promotions_player_vip_level_snapshot_id");
        builder.HasIndex(b => new { b.VipId, b.PlayerCode })
            .HasDatabaseName("ix_player_vip_level_upgrade_promotions_vip_id_player_code");
    }
}
