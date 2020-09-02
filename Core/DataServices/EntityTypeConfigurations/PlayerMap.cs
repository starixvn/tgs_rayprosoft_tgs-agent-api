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

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.HasMany(p => p.Wallets)
                .WithOne(p => p.Player)
                .HasForeignKey(p => p.PlayerCode);

            builder.Property(c => c.Username)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(c => c.IpAddress)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(c => c.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder.Property(c => c.BetLimitGroupId)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(c => c.DefaultWalletCurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.LastActiveOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.LastLoginUrl)
                .HasColumnType(ColumnTypes.Nvarchar(1024));

            builder.Property(c => c.LastCashierUrl)
                .HasColumnType(ColumnTypes.Nvarchar(1024));

            builder.Property(c => c.LastHelpUrl)
                .HasColumnType(ColumnTypes.Nvarchar(1024));

            builder.Property(c => c.LastTermsUrl)
                .HasColumnType(ColumnTypes.Nvarchar(1024));
        }
    }
}