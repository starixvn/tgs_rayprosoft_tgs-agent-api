using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AgentLoginRecordMap : IEntityTypeConfiguration<AgentLoginRecord>
    {
        void IEntityTypeConfiguration<AgentLoginRecord>.Configure(EntityTypeBuilder<AgentLoginRecord> builder)
        {
            builder.ToTable("agent_login_records");

            builder.HasKey(b => b.Id)
                .HasName("pk_agent_login_records");

            builder.Property(b => b.AgentId)
                .IsRequired();

            builder.Property(b => b.BrandId)
                .IsRequired();

            builder.Property(b => b.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.Property(b => b.ExpiredOn)
                .IsRequired();

            builder.HasOne(b => b.Agent)
                .WithMany()
                .HasForeignKey(b => b.AgentId)
                .HasConstraintName("fk_agent_login_records_agent_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.AgentId)
                .HasDatabaseName("ix_agent_login_records_agent_id");

            builder.HasOne(b => b.Brand)
                .WithMany()
                .HasForeignKey(b => b.BrandId)
                .HasConstraintName("fk_agent_login_records_brands_brand_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.BrandId)
                .HasDatabaseName("ix_agent_login_records_brand_id");
        }
    }
}