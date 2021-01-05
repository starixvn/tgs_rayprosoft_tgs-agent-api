using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BrandPaymentPlayerMap : IEntityTypeConfiguration<BrandPaymentPlayer>
    {
        void IEntityTypeConfiguration<BrandPaymentPlayer>.Configure(
            EntityTypeBuilder<BrandPaymentPlayer> builder)
        {
            builder.ToTable("BrandPaymentPlayers", "dbo");

            builder.HasKey(p => new {p.BrandPaymentId, p.PlayerCode});

            builder.HasOne(p => p.BrandPayment)
                .WithMany(u => u.BrandPaymentPlayers)
                .HasForeignKey(u => u.BrandPaymentId);

            builder.HasOne(p => p.Player)
                .WithMany(u => u.BrandPaymentPlayers)
                .HasForeignKey(u => u.PlayerCode);
        }
    }
}