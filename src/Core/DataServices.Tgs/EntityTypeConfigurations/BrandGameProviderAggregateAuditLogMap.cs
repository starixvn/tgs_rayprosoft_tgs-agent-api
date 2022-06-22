using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class BrandGameProviderAggregateAuditLogMap : IEntityTypeConfiguration<BrandGameProviderAggregateAuditLog>
{
    void IEntityTypeConfiguration<BrandGameProviderAggregateAuditLog>.Configure(
        EntityTypeBuilder<BrandGameProviderAggregateAuditLog> builder)
    {
        builder.ToTable("brand_game_provider_aggregate_audit_logs");

        builder.HasKey(c => c.Id)
            .HasName("pk_brand_game_provider_aggregate_audit_log_id");

        builder.Property(c => c.CreatedByUserId)
            .IsRequired();

        builder.Property(c => c.CreatedOn)
            .IsRequired();

        builder.HasIndex(c => new
            {
                c.CreatedOn,
                c.CreatedByUserId,
                c.BrandId
            })
            .HasDatabaseName("ix_brand_game_provider_aggregate_audit_log_multiple_keys");
    }
}