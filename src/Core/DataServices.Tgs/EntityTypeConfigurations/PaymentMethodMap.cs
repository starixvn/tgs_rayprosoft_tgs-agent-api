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

            builder.Property(c => c.BankType)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(128);
        }
    }
}
