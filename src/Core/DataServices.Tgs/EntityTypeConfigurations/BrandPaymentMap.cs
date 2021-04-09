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
            builder.ToTable("brand_payments");

            builder.HasKey(p => p.Id)
                .HasName("pk_brand_payments");

            builder.Property(c => c.ChannelName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Reason)
                .HasMaxLength(4000);

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.Property(c => c.TransferLimitFrom)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.TransferLimitTo)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.DepositedAmountMin)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.DepositedAmountMax)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.FeePercentage)
                .HasPrecision(18, 6);

            builder.Property(c => c.LowestFee)
                .HasPrecision(18, 6);

            builder.Property(c => c.HighestFee)
                .HasPrecision(18, 6);

            builder.Property(c => c.ExtendedData)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.PaymentMethodChannelId)
                .IsRequired();

            builder.HasOne(c => c.PaymentMethodChannel)
                .WithMany()
                .HasForeignKey(c => c.PaymentMethodChannelId)
                .HasConstraintName("fk_brand_payments_payment_method_channels_id");

            builder.HasIndex(c => c.PaymentMethodChannelId)
                .HasDatabaseName("ix_brand_payments_payment_method_channel_id");

            builder.HasOne(c => c.Brand)
                .WithMany()
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_brand_payments_brands_brand_id");
        }
    }
}