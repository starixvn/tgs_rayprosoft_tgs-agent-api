using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerDailyPaymentStatisticsMap : BaseConfiguration, IEntityTypeConfiguration<PlayerDailyPaymentStatistics>
{
    public void Configure(EntityTypeBuilder<PlayerDailyPaymentStatistics> builder)
    {
        builder.ToTable("player_daily_payment_statistics");

        builder.HasKey(p => new { p.ReportDate, p.PlayerCode });

        builder.Property(p => p.ReportDate)
            .IsRequired();
        builder.Property(p => p.PlayerCode)
            .IsRequired();

        builder.Property(p => p.DailyDepositCount);
        builder.Property(p => p.DailyDepositAmount);
        builder.Property(p => p.TotalDepositCount);
        builder.Property(p => p.TotalDepositAmount);
        builder.Property(p => p.DailyWithdrawCount);
        builder.Property(p => p.DailyWithdrawAmount);
        builder.Property(p => p.TotalWithdrawAmount);
        builder.Property(p => p.TotalWithdrawCount);
    }
}
