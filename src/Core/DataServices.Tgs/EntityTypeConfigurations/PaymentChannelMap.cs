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
        }
    }
}