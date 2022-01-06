using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerVipLevelSnapshotMap : IEntityTypeConfiguration<PlayerVipLevelSnapshot>
    {
        void IEntityTypeConfiguration<PlayerVipLevelSnapshot>.Configure(EntityTypeBuilder<PlayerVipLevelSnapshot> builder)
        {
            builder.ToTable("player_vip_level_snapshots");

            builder.HasKey(b => b.Id)
                .HasName("pk_player_vip_level_snapshots");
            builder.Property(b => b.Id)
            .IsRequired();
            builder.Property(b => b.BrandId)
                .IsRequired();
            builder.Property(b => b.SnapshotOn)
                .IsRequired();
            builder.Property(c => c.Data)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();
            builder.Property(b => b.CreatedOn)
                .IsRequired();
            builder.HasIndex(b =>  b.BrandId)
                .HasDatabaseName("ix_player_vip_level_snapshots_brand_id");
        }
    }
}
