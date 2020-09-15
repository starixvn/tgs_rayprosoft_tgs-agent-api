using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class GameActionMap : IEntityTypeConfiguration<GameAction>
    {
        void IEntityTypeConfiguration<GameAction>.Configure(EntityTypeBuilder<GameAction> builder)
        {
            builder.ToTable("GameActions", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);

            builder.Property(c => c.ExternalBetId)
                .HasColumnType(ColumnTypes.Nvarchar(64));

            builder.HasMany(p => p.FollowUpGameActions)
                .WithOne(p => p.ReferencedGameAction)
                .HasForeignKey(p => p.ReferencedGameActionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.IsClosingRound)
                .IsRequired();

            builder.Property(c => c.ExternalTransactionId)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.RoundId)
                .IsRequired();

            builder.Property(c => c.WalletId)
                .IsRequired();

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.LicenseeId)
                .IsRequired();

            builder.Property(c => c.RecordedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.SentOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired(false);

            // shadow property, no actual property in GameAction model
            builder.Property<DateTimeOffset>("StoredOn")
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .IsRequired();

            builder.Property(c => c.Turnover)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.WinLoss)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.UnsettledBets)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.Balance)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.PlayerIp)
                .HasColumnType(ColumnTypes.Varchar(128))
                .IsRequired();

            builder.Property(c => c.ExternalGameActionGroupId)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(c => c.BetType)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();
        }
    }
}
