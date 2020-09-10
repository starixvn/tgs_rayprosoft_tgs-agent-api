using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class RoundMap : IEntityTypeConfiguration<Round>
    {
        void IEntityTypeConfiguration<Round>.Configure(EntityTypeBuilder<Round> builder)
        {
            builder.ToTable("Rounds", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);

            builder.HasMany(p => p.GameActions)
                .WithOne(p => p.Round)
                .HasForeignKey(p => p.RoundId);

            builder.HasOne(p => p.RoundDetail)
                .WithOne(p => p.Round)
                .HasForeignKey<RoundDetail>(p => p.RoundId);
                        
            builder.Property(c => c.ExternalRoundId)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameCode)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.GameName)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameProviderCode)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.GameProviderName)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.PlayerTypes)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.CurrencyCode)
                .HasColumnType(ColumnTypes.Varchar(10))
                .IsRequired();

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.WalletId)
                .IsRequired();

            builder.Property(c => c.GameId)
                .IsRequired();

            builder.Property(c => c.LicenseeId)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.ClosedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset);

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.ExternalCreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset);

            builder.Property(c => c.ExternalClosedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset);

            builder.Property(c => c.BetAmount)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

             builder.Property(c => c.WinAmount)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.WinLoss)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.TiebetAdjustment)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.Commission)
                .HasColumnType(ColumnTypes.Decimal(18, 6));
        }
    }
}
