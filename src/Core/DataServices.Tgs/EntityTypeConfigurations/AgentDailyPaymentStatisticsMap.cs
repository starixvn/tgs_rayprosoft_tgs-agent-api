using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class AgentDailyPaymentStatisticsMap : BaseConfiguration, IEntityTypeConfiguration<AgentDailyPaymentStatistics>
{
    public void Configure(EntityTypeBuilder<AgentDailyPaymentStatistics> builder)
    {
        builder.ToTable("agent_daily_payment_statistics");

        builder.HasKey(a => new { a.ReportDate, a.BrandId, a.AgentId });

        builder.Property(a => a.ReportDate)
            .IsRequired();
        builder.Property(a => a.BrandId)
            .IsRequired();
        builder.Property(a => a.AgentId)
            .IsRequired();

        builder.Property(a => a.FirstDepositPlayers)
            .IsRequired();
        builder.Property(a => a.UndepositAgainPlayers)
            .IsRequired();
        builder.Property(a => a.DepositPlayers)
            .IsRequired();
    }
}
