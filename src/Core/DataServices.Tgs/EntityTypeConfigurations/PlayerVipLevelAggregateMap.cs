using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerVipLevelAggregateMap : IEntityTypeConfiguration<PlayerVipLevelAggregate>
    {
        void IEntityTypeConfiguration<PlayerVipLevelAggregate>.Configure(EntityTypeBuilder<PlayerVipLevelAggregate> builder)
        {
            builder.ToTable("player_vip_level_aggregates");

            builder.HasKey(b => new { b.BrandId, b.PlayerCode, b.FromOn, b.ToOn })
                .HasName("pk_player_vip_level_aggregates");
            builder.Property(b => b.BrandId)
                .IsRequired();
            builder.Property(b => b.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();
            builder.Property(b => b.FromOn)
                .IsRequired();
            builder.Property(b => b.ToOn)
                .IsRequired();
            builder.Property(b => b.DepositAmount)
                .HasPrecision(18, 6)
                .IsRequired();
            builder.Property(b => b.Turnover)
                .HasPrecision(18, 6)
                .IsRequired();
            builder.Property(b => b.CreatedOn)
                .IsRequired();
            builder.Property(b => b.UpdateOn)
                .IsRequired();

            builder.HasIndex(b => new { b.PlayerCode, b.FromOn, b.ToOn })
                .HasDatabaseName("ix_player_vip_level_aggregates_player_code");
        }
    }
}
