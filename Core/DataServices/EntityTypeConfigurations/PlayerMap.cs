using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class PlayerMap : IEntityTypeConfiguration<Player>
    {
        void IEntityTypeConfiguration<Player>.Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players", "dbo");
            builder.HasKey(bc => bc.PlayerCode);
            builder.HasIndex(p => p.Username);

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.ParentPlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired(false);

            builder.Property(c => c.Username)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.RealName)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired(false);

            builder.Property(c => c.InvitationCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder.Property(c => c.ParentInvitationCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired(false);

            builder.Property(c => c.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder.Property(c => c.Password)
                .HasColumnType(ColumnTypes.Varbinary(128))
                .IsRequired();

            builder.Property(c => c.Salt)
                .HasColumnType(ColumnTypes.Varbinary(128))
                .IsRequired();

            builder.Property(c => c.BetLimitGroupId)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(c => c.DefaultWalletCurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(c => c.RegisteredOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.LastActiveOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.VipId)
                .IsRequired(false);

            builder.Property(c => c.Phone)
                .HasColumnType(ColumnTypes.Nvarchar(32));

            builder.Property(c => c.Email)
                .HasColumnType(ColumnTypes.Nvarchar(64));

            builder.Property(c => c.QQ)
                .HasColumnType(ColumnTypes.Nvarchar(64));

            builder.Property(c => c.Wechat)
                .HasColumnType(ColumnTypes.Nvarchar(64));

            builder.Property(c => c.AccumulatedDeposit)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasMany(p => p.Wallets)
                .WithOne(p => p.Player)
                .HasForeignKey(p => p.PlayerCode);

            builder.HasMany(p => p.Banks)
                .WithOne(p => p.Player)
                .HasForeignKey(p => p.PlayerCode);

            builder.HasOne(p => p.Vip)
                .WithMany(p => p.Players)
                .HasForeignKey(p => p.VipId);

            builder.HasOne(p => p.ParentPlayer)
                .WithMany(p => p.SubPlayers)
                .HasForeignKey(p => p.ParentPlayerCode)
                .IsRequired(false);

            builder.HasMany(p => p.SubPlayers)
                .WithOne(p => p.ParentPlayer)
                .HasForeignKey(p => p.ParentPlayerCode);

            builder.HasMany(p => p.Letters)
                .WithOne(p => p.Player)
                .HasForeignKey(p => p.PlayerCode);
        }
    }
}