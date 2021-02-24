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

            //builder.Property(c => c.BeforeChannelName)
            //    .HasMaxLength(50);

            //builder.Property(c => c.AfterChannelName)
            //    .HasMaxLength(50)
            //    .IsRequired();

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

            //builder.Property(c => c.BeforeTransferLimitFrom)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.BeforeTransferLimitTo)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.BeforeDepositedAmountMin)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.BeforeDepositedAmountMax)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.AfterTransferLimitFrom)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.AfterTransferLimitTo)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.AfterDepositedAmountMin)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            //builder.Property(c => c.AfterDepositedAmountMax)
            //    .HasPrecision(18, 6)
            //    .IsRequired();

            builder.Property(c => c.Before)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.After)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();
        }
    }
}