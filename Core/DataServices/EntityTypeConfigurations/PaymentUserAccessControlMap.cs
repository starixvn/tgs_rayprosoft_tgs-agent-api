using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class PaymentUserAccessControlMap : IEntityTypeConfiguration<PaymentUserAccessControl>
    {
        void IEntityTypeConfiguration<PaymentUserAccessControl>.Configure(
            EntityTypeBuilder<PaymentUserAccessControl> builder)
        {
            builder.ToTable("PaymentUserAccessControls", "dbo");

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Payment)
                .WithMany(u => u.PaymentUserAccessControls)
                .HasForeignKey(u => u.PaymentId);

            builder.HasOne(p => p.Player)
                .WithMany(u => u.PaymentUserAccessControls)
                .HasForeignKey(u => u.PlayerCode);
        }
    }
}