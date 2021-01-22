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
            builder.ToTable("rounds");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_rounds");

            builder.Property(c => c.ExternalRoundId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameId)
                .IsRequired();

            builder.Property(c => c.GameName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameProviderId)
                .IsRequired();

            builder.Property(c => c.GameProviderCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.GameProviderName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.PlayerType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.WalletId)
                .IsRequired();

            builder.Property(c => c.ResellerId)
                .IsRequired();

            builder.Property(c => c.ResellerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.LicenseeId)
                .IsRequired();

            builder.Property(c => c.LicenseeCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.BrandCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.ClosedOn);

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.Property(c => c.ExternalCreatedOn);

            builder.Property(c => c.ExternalClosedOn);

            builder.Property(c => c.BetAmount)
                .HasPrecision(18, 6);

            builder.Property(c => c.WinLoss)
                .HasPrecision(18, 6);

            builder.Property(c => c.TieBetAdjustment)
                .HasPrecision(18, 6);

            builder.Property(c => c.Commission)
                .HasPrecision(18, 6);
        }
    }
}