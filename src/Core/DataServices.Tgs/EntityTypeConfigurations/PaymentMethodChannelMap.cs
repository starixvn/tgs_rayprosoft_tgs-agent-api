using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PaymentMethodChannelMap : IEntityTypeConfiguration<PaymentMethodChannel>
    {
        public void Configure(EntityTypeBuilder<PaymentMethodChannel> builder)
        {
            builder.ToTable("payment_method_channels");

            builder.HasKey(p => p.Id)
                .HasName("pk_payment_method_channels");

            builder.Property(p => p.PaymentId)
                .IsRequired();

            builder.Property(p => p.MethodId)
                .IsRequired();

            builder.Property(p => p.ChannelId)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValueSql("0")
                .IsRequired();

            builder
                .HasOne(c => c.Payment)
                .WithMany()
                .HasForeignKey(c => c.PaymentId)
                .HasConstraintName("fk_payment_method_channels_payments_id");

            builder.HasIndex(c => c.PaymentId)
                .HasDatabaseName("ix_payment_method_channels_payment_id");

            builder
                .HasOne(c => c.Method)
                .WithMany()
                .HasForeignKey(c => c.MethodId)
                .HasConstraintName("fk_payment_method_channels_payment_methods_id");

            builder.HasIndex(c => c.MethodId)
                .HasDatabaseName("ix_payment_method_channels_method_id");

            builder
                .HasOne(c => c.Channel)
                .WithMany()
                .HasForeignKey(c => c.ChannelId)
                .HasConstraintName("fk_payment_method_channels_payment_channels_id");

            builder.HasIndex(c => c.ChannelId)
                .HasDatabaseName("ix_payment_method_channels_channel_id");

            builder.HasAlternateKey(c => new { c.PaymentId, c.MethodId, c.ChannelId })
                .HasName("ak_payment_method_channels_payment_id_method_id_channed_id");
        }
    }
}
