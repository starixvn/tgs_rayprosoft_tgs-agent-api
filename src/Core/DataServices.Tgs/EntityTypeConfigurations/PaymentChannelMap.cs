using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class PaymentChannelMap : IEntityTypeConfiguration<PaymentChannel>
    {
        void IEntityTypeConfiguration<PaymentChannel>.Configure(EntityTypeBuilder<PaymentChannel> builder)
        {
            builder.ToTable("payment_channels");

            builder.HasKey(c => c.Id)
                .HasName("pk_payment_channels");

            builder.Property(c => c.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(128);

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

            builder.HasOne(c => c.Payment)
                .WithMany(c => c.PaymentChannels)
                .HasForeignKey(c => c.PaymentId)
                .HasConstraintName("fk_payment_channels_payments_payment_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.PaymentId)
                .HasDatabaseName("ix_payment_channels_payment_id");

            builder.HasOne(c => c.PaymentMethod)
                .WithMany(c => c.PaymentChannels)
                .HasForeignKey(c => c.PaymentMethodId)
                .HasConstraintName("fk_payment_channels_payment_methods_payment_method_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.PaymentMethodId)
                .HasDatabaseName("ix_payment_channels_payment_method_id");
        }
    }
}