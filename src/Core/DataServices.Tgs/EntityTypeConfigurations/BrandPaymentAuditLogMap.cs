using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BrandPaymentAuditLogMap : IEntityTypeConfiguration<BrandPaymentAuditLog>
    {
        void IEntityTypeConfiguration<BrandPaymentAuditLog>.Configure(EntityTypeBuilder<BrandPaymentAuditLog> builder)
        {
            builder.ToTable("brand_payment_audit_logs");

            builder.HasKey(p => p.Id)
                .HasName("pk_brand_payment_audit_logs");

            builder.HasIndex(p => p.BrandPaymentId);

            builder.Property(c => c.Reason)
                .HasMaxLength(4000);

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.Operation)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Before)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.After)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();
        }
    }
}
