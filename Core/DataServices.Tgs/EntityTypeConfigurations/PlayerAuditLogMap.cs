using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class PlayerAuditLogMap : IEntityTypeConfiguration<PlayerAuditLog>
    {
        void IEntityTypeConfiguration<PlayerAuditLog>.Configure(EntityTypeBuilder<PlayerAuditLog> builder)
        {
            builder.ToTable("player_audit_logs");

            builder.HasKey(c => c.Id)
                .HasName("pk_player_audit_logs");

            builder.Property(c => c.Username)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.ModuleName)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Operation)
                .HasMaxLength(256)
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .IsRequired();
        }
    }
}