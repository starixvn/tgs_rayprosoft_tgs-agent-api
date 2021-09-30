using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AgentDividendConditionMap : IEntityTypeConfiguration<AgentDividendCondition>
    {
        void IEntityTypeConfiguration<AgentDividendCondition>.Configure(
            EntityTypeBuilder<AgentDividendCondition> builder)
        {
            builder.ToTable("agent_dividend_conditions");

            builder.HasKey(b => new
                {
                    b.Rank,
                    b.AgentId
                })
                .HasName("pk_agent_dividend_conditions_rank_agent_id");

            builder.Property(b => b.NetProfit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(b => b.DividendPercentage)
                .HasPrecision(18, 6)
                .IsRequired();
        }
    }
}