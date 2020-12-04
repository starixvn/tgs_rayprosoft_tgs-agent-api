using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeGameProviderMap : IEntityTypeConfiguration<LicenseeGameProvider>
    {
        void IEntityTypeConfiguration<LicenseeGameProvider>.Configure(EntityTypeBuilder<LicenseeGameProvider> builder)
        {
            builder.ToTable("LicenseeGameProviders", "dbo");

            builder.HasKey(lgp => new { lgp.LicenseeId, lgp.GameProviderId });

            builder
                .HasOne(lgp => lgp.GameProvider)
                .WithMany(gp => gp.Licensees)
                .HasForeignKey(bgp => bgp.GameProviderId);

            builder
                .HasOne(lgp => lgp.Licensee)
                .WithMany(l => l.GameProviders)
                .HasForeignKey(rgp => rgp.LicenseeId);
        }
    }
}
