using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class AgentAuditLogMap : IEntityTypeConfiguration<AgentAuditLog>
    {
        void IEntityTypeConfiguration<AgentAuditLog>.Configure(EntityTypeBuilder<AgentAuditLog> builder)
        {
            builder.ToTable("agent_audit_logs");

            builder.HasKey(c => c.Id)
                .HasName("pk_agent_audit_logs");

            builder.Property(c => c.Operation)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasOne(c => c.Agent)
                .WithMany()
                .HasForeignKey(c => c.AgentId)
                .HasConstraintName("fk_agent_audit_logs_agent_id");
        }
    }
}