using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderAggregateAuditLogMap : IEntityTypeConfiguration<GameProviderAggregateAuditLog>
    {
        void IEntityTypeConfiguration<GameProviderAggregateAuditLog>.Configure(
            EntityTypeBuilder<GameProviderAggregateAuditLog> builder)
        {
            builder.ToTable("game_provider_aggregate_audit_logs");

            builder.HasKey(c => c.Id)
                .HasName("pk_game_provider_aggregate_audit_log_id");

            builder.Property(c => c.CreatedByUserId)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => new { c.CreatedOn })
                .HasDatabaseName("ix_game_provider_aggregate_audit_log_created_on");
        }
    }
}