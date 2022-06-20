using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class AgentDailyAggregateMap : IEntityTypeConfiguration<AgentDailyAggregate>
{
    public void Configure(EntityTypeBuilder<AgentDailyAggregate> builder)
    {
        _ = builder.ToTable("agent_daily_aggregates", Consts.ReportSchema);

        _ = builder.HasKey(c => new { c.ReportDate, c.AgentId })
            .HasName("pk_agent_daily_aggregate_report_date_agent_id");

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
