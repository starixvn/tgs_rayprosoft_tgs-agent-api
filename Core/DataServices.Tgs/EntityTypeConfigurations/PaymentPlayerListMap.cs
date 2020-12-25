using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class PaymentPlayerListMap : IEntityTypeConfiguration<PaymentPlayerList>
    {
        void IEntityTypeConfiguration<PaymentPlayerList>.Configure(
            EntityTypeBuilder<PaymentPlayerList> builder)
        {
            builder.ToTable("PaymentPlayerList", "dbo");

            builder.HasKey(p => new {p.PaymentId, p.PlayerCode});

            builder.HasOne(p => p.Payment)
                .WithMany(u => u.PaymentPlayerList)
                .HasForeignKey(u => u.PaymentId);

            builder.HasOne(p => p.Player)
                .WithMany(u => u.PaymentPlayerList)
                .HasForeignKey(u => u.PlayerCode);
        }
    }
}