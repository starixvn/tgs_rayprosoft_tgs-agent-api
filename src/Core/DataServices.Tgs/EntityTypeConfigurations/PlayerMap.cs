using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerMap : IEntityTypeConfiguration<Player>
    {
        void IEntityTypeConfiguration<Player>.Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("players");

            builder.HasKey(bc => bc.PlayerCode)
                .HasName("pk_player_code");

            builder.Property(c => c.IdNumber)
                .HasMaxLength(50);

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.ParentPlayerCode)
                .HasMaxLength(16);

            builder.Property(c => c.Username)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.RealName)
                .HasMaxLength(32);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.InvitationCode)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(c => c.ParentInvitationCode)
                .HasMaxLength(6);

            builder.Property(c => c.Password)
                .IsRequired();

            builder.Property(c => c.Salt)
                .IsRequired();

            builder.Property(c => c.WithdrawPassword)
                .IsRequired();

            builder.Property(c => c.WithdrawSalt)
                .IsRequired();

            builder.Property(c => c.BetLimitId)
                .IsRequired();

            builder.Property(c => c.DefaultWalletCurrencyCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.RegisteredOn)
                .IsRequired();

            builder.Property(c => c.LastActiveOn)
                .IsRequired();

            builder.Property(c => c.VipId);

            builder.Property(c => c.Phone)
                .HasMaxLength(32);

            builder.Property(c => c.Email)
                .HasMaxLength(64);

            builder.Property(c => c.QQ)
                .HasMaxLength(64);

            builder.Property(c => c.Wechat)
                .HasMaxLength(64);

            builder.Property(c => c.AccumulatedDeposit)
                .HasPrecision(18, 6)
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasOne(p => p.Brand)
                .WithMany(p => p.Players)
                .HasForeignKey(p => p.BrandId)
                .HasConstraintName("fk_players_brands_brand_id");

            builder.HasOne(p => p.Vip)
                .WithMany(p => p.Players)
                .HasForeignKey(p => p.VipId)
                .HasConstraintName("fk_players_vip_levels_vip_id");

            builder.HasIndex(p => p.VipId)
                .HasDatabaseName("ix_players_vip_id");

            builder.HasOne(p => p.ParentPlayer)
                .WithMany(p => p.SubPlayers)
                .HasForeignKey(p => p.ParentPlayerCode)
                .HasConstraintName("fk_players_parent_player_code")
                .IsRequired(false);

            builder.HasOne(p => p.BetLimit)
                .WithMany(p => p.Players)
                .HasForeignKey(p => p.BetLimitId)
                .HasConstraintName("fk_players_bet_limits_bet_limit_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.BetLimitId)
                .HasDatabaseName("ix_players_bet_limit_id");
        }
    }
}