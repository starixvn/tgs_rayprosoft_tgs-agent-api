using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Report.Models;

namespace TGS.Core.DataServices.Report.EntityTypeConfigurations
{
    internal class MarketMap : IEntityTypeConfiguration<Market>
    {
        void IEntityTypeConfiguration<Market>.Configure(EntityTypeBuilder<Market> builder)
        {
            builder.ToTable("markets");

            builder.HasKey(b => b.Id)
                .HasName("pk_markets");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.RegisteredCount)
                .IsRequired();

            builder.Property(b => b.FirstFundInCount)
                .IsRequired();

            builder.Property(b => b.DepositAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.WithdrawalAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.AdjustmentAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.BonusAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.BetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.ValidBetAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.WinLossAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.NetProfit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();
        }
    }
}