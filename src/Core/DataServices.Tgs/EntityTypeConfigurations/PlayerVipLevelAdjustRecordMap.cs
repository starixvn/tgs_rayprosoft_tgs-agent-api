using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerVipLevelAdjustRecordMap : IEntityTypeConfiguration<PlayerVipLevelAdjustRecord>
{
    void IEntityTypeConfiguration<PlayerVipLevelAdjustRecord>.Configure(EntityTypeBuilder<PlayerVipLevelAdjustRecord> builder)
    {
        builder.ToTable("player_vip_level_adjust_records");

        builder.HasKey(b => b.Id)
            .HasName("pk_player_vip_level_adjust_records");

        builder.Property(b => b.Id)
            .IsRequired();
        builder.Property(b => b.PlayerCode)
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(b => b.OldVipId)
            .IsRequired();
        builder.Property(b => b.NewVipId)
            .IsRequired();
        builder.Property(b => b.OperatorId)
            .IsRequired();
        builder.Property(b => b.Remark)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(b => b.CreatedOn)
            .IsRequired();

        builder.HasIndex(b => b.PlayerCode)
            .HasDatabaseName("ix_player_vip_level_adjust_records_player_code");

        builder.HasIndex(b => b.CreatedOn)
.HasDatabaseName("ix_player_vip_level_adjust_records_created_on");
    }
}
