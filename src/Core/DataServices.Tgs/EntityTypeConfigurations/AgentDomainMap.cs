using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class AgentDomainMap : IEntityTypeConfiguration<AgentDomain>
    {
        public void Configure(EntityTypeBuilder<AgentDomain> builder)
        {
            builder.ToTable("agent_domains");

            builder.HasKey(d => new { d.AgentId, d.DomainName });

            builder.Property(d => d.AgentId)
                .IsRequired();

            builder.Property(d => d.DomainName)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasOne<Agent>()
                .WithMany()
                .HasForeignKey(d => d.AgentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_agent_domains_agents_agent_id");
        }
    }
}
