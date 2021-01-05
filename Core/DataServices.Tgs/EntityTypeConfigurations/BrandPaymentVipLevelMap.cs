using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandPaymentVipLevelMap : IEntityTypeConfiguration<BrandPaymentVipLevel>
    {
        void IEntityTypeConfiguration<BrandPaymentVipLevel>.Configure(EntityTypeBuilder<BrandPaymentVipLevel> builder)
        {
            builder.ToTable("BrandPaymentVipLevels", "dbo");
            builder.HasKey(bc => new {PaymentId = bc.BrandPaymentId, bc.VipLevelId});

            builder
                .HasOne(bc => bc.BrandPayment)
                .WithMany(c => c.VipLevels)
                .HasForeignKey(bc => bc.BrandPaymentId);

            builder
                .HasOne(bc => bc.VipLevel)
                .WithMany(c => c.PaymentVipLevels)
                .HasForeignKey(bc => bc.VipLevelId);
        }
    }
}