using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class AgentDailyMarketingStatisticsMap : IEntityTypeConfiguration<AgentDailyMarketingStatistics>
{
    public void Configure(EntityTypeBuilder<AgentDailyMarketingStatistics> builder)
    {
        builder.ToTable("agent_daily_marketing_statistics");

        builder.HasKey(adms => new { adms.ReportDate, adms.BrandId, adms.AgentId })
            .HasName("pk_agent_daily_marketing_statistics");

        builder.Property(adms => adms.ReportDate)
            .IsRequired();
        builder.Property(adms => adms.BrandId)
            .IsRequired();
        builder.Property(adms => adms.AgentId)
            .IsRequired();

        builder.Property(adms => adms.RegisterCount)
            .IsRequired();
        builder.Property(adms => adms.GamingCount)
            .IsRequired();
    }
}
