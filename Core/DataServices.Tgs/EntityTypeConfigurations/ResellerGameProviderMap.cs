using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ResellerGameProviderMap : IEntityTypeConfiguration<ResellerGameProvider>
    {
        void IEntityTypeConfiguration<ResellerGameProvider>.Configure(EntityTypeBuilder<ResellerGameProvider> builder)
        {
            builder.ToTable("ResellerGameProviders", "dbo");

            builder.HasKey(rgp => new { rgp.ResellerId, rgp.GameProviderId });

            builder
                .HasOne(rgp => rgp.GameProvider)
                .WithMany(gp => gp.Resellers)
                .HasForeignKey(bgp => bgp.GameProviderId);

            builder
                .HasOne(rgp => rgp.Reseller)
                .WithMany(r => r.GameProviders)
                .HasForeignKey(rgp => rgp.ResellerId);
        }
    }
}
