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
            builder.ToTable("BrandPaymentAuditLogs", "dbo");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.BrandPaymentId);

            builder.Property(c => c.BeforeChannelName)
                .HasColumnType(ColumnTypes.Nvarchar(50));

            builder.Property(c => c.AfterChannelName)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired();

            builder.Property(c => c.Reason)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();


            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.Operation)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.BeforeTransferLimitFrom)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.BeforeTransferLimitTo)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.BeforeDepositedAmountMin)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.BeforeDepositedAmountMax)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.AfterTransferLimitFrom)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.AfterTransferLimitTo)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.AfterDepositedAmountMin)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.AfterDepositedAmountMax)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();
        }
    }
}