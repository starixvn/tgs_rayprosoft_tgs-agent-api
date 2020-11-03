using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class PaymentPlayerListMap : IEntityTypeConfiguration<PaymentPlayerList>
    {
        void IEntityTypeConfiguration<PaymentPlayerList>.Configure(
            EntityTypeBuilder<PaymentPlayerList> builder)
        {
            builder.ToTable("PaymentUserAccessControls", "dbo");

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Payment)
                .WithMany(u => u.PaymentPlayerList)
                .HasForeignKey(u => u.PaymentId);

            builder.HasOne(p => p.Player)
                .WithMany(u => u.PaymentPlayerList)
                .HasForeignKey(u => u.PlayerCode);
        }
    }
}