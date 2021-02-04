using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class PaymentMethodMap : IEntityTypeConfiguration<PaymentMethod>
    {
        void IEntityTypeConfiguration<PaymentMethod>.Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("payment_methods");

            builder.HasKey(p => p.Id)
                .HasName("pk_payment_methods");

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
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(c => c.PaymentId)
                .HasConstraintName("fk_payment_methods_payments_payment_id");

            builder.HasIndex(c => c.PaymentId)
                .HasDatabaseName("ix_payment_methods_payment_id");
        }
    }
}