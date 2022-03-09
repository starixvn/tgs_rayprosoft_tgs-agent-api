using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGameProviderAggregateRecordMap : IEntityTypeConfiguration<BrandGameProviderAggregateRecord>
    {
        void IEntityTypeConfiguration<BrandGameProviderAggregateRecord>.Configure(
            EntityTypeBuilder<BrandGameProviderAggregateRecord> builder)
        {
            builder.ToTable("brand_game_provider_aggregate_records", Consts.ReportSchema);

            builder.HasKey(c => c.Id)
                .HasName("pk_brand_game_provider_aggregate_record_id");

            builder.Property(c => c.AuditLogId)
                .IsRequired();

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.Data)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => c.AuditLogId)
                .HasDatabaseName("ix_brand_game_provider_aggregate_record_audit_log_id");
        }
    }
}