using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PaymentVipLevelMap : IEntityTypeConfiguration<PaymentVipLevel>
    {
        void IEntityTypeConfiguration<PaymentVipLevel>.Configure(EntityTypeBuilder<PaymentVipLevel> builder)
        {
            builder.ToTable("PaymentVipLevels", "dbo");
            builder.HasKey(bc => new {bc.PaymentId, bc.VipLevelId});

            builder
                .HasOne(bc => bc.Payment)
                .WithMany(c => c.VipLevels)
                .HasForeignKey(bc => bc.PaymentId);

            builder
                .HasOne(bc => bc.VipLevel)
                .WithMany(c => c.PaymentVipLevels)
                .HasForeignKey(bc => bc.VipLevelId);
        }
    }
}