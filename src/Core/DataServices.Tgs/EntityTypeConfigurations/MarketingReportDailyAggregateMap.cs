using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class MarketingReportDailyAggregateMap : IEntityTypeConfiguration<MarketingReportDailyAggregate>
{
    public void Configure(EntityTypeBuilder<MarketingReportDailyAggregate> builder)
    {
        _ = builder.ToTable("marketing_report_daily_aggregates", Consts.ReportSchema);

        _ = builder.HasKey(c => new { c.ReportDate, c.AgentId })
            .HasName("pk_marketing_report_daily_aggregates_report_date_agent_id");

        _ = builder.Property(c => c.AgentId)
            .IsRequired();

        _ = builder.Property(c => c.ReportDate)
            .IsRequired();

        _ = builder.Property(c => c.RegisterCount)
            .HasDefaultValue(0)
            .IsRequired();

        _ = builder.Property(c => c.FirstDepositCount)
            .HasDefaultValue(0)
            .IsRequired();

        _ = builder.Property(c => c.GamingCount)
            .HasDefaultValue(0)
            .IsRequired();
    }
}
