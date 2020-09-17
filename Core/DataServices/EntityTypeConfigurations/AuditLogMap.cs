using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public sealed class AuditLogMap : IEntityTypeConfiguration<AuditLog>
    {
        void IEntityTypeConfiguration<AuditLog>.Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "dbo");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
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
