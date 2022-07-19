using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class BrandDailyPaymentStatisticsMap : BaseConfiguration, IEntityTypeConfiguration<BrandDailyPaymentStatistics>
{
    public void Configure(EntityTypeBuilder<BrandDailyPaymentStatistics> builder)
    {
        builder.ToTable("brand_daily_payment_statistics");

        builder.HasKey(b => new { b.ReportDate, b.BrandId });

        builder.Property(b => b.ReportDate)
            .IsRequired();
        builder.Property(b => b.BrandId)
            .IsRequired();
        builder.Property(b => b.FirstDepositPlayers)
            .IsRequired();
        builder.Property(b => b.UndepositAgainPlayers)
            .IsRequired();
        builder.Property(b => b.DepositPlayers)
            .IsRequired();
    }
}
