using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BrandPaymentMap : IEntityTypeConfiguration<BrandPayment>
    {
        void IEntityTypeConfiguration<BrandPayment>.Configure(EntityTypeBuilder<BrandPayment> builder)
        {
            builder.ToTable("BrandPayments", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.ChannelName)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired();

            builder.Property(c => c.Reason)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.TransferLimitFrom)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.TransferLimitTo)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.DepositedAmountMin)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.DepositedAmountMax)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(c => c.FeePercentage)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.LowestFee)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder.Property(c => c.HighestFee)
                .HasColumnType(ColumnTypes.Decimal(18, 6));
        }
    }
}