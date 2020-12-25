using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class AuditLogMap : IEntityTypeConfiguration<PlayerAuditLog>
    {
        void IEntityTypeConfiguration<PlayerAuditLog>.Configure(EntityTypeBuilder<PlayerAuditLog> builder)
        {
            builder.ToTable("PlayerAuditLogs", "dbo");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Username)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired(false);

            builder.Property(c => c.ModuleName)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Operation)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

        }
    }
}
