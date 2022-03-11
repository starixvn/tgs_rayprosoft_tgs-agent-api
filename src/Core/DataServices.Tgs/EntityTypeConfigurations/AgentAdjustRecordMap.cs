using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AgentAdjustRecordMap : IEntityTypeConfiguration<AdjustAgentRecord>
    {
        void IEntityTypeConfiguration<AdjustAgentRecord>.Configure(EntityTypeBuilder<AdjustAgentRecord> builder)
        {
            builder.ToTable("agent_adjust_records");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_agent_adjust_records_id");

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.AgentId)
                .IsRequired();

            builder.Property(c => c.AdjustmentType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.TransferType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(p => p.TransferTarget);

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6);

            builder.Property(p => p.Remark);

            builder.Property(l => l.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => new
                {
                    c.CreatedOn,
                    c.BrandId,
                    c.AgentId,
                    c.Type
                })
                .HasDatabaseName("ix_agent_adjust_record_multiple_keys");
        }
    }
}