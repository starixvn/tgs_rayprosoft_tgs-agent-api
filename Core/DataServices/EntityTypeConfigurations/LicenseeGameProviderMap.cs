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

            builder.HasKey(bc => new { bc.LicenseeId, bc.GameProviderId });
            builder
                .HasOne(bc => bc.GameProvider)
                .WithMany(c => c.Licensee)
                .HasForeignKey(bc => bc.GameProviderId);

            builder
                .HasOne(bc => bc.Licensee)
                .WithMany(c => c.GameProviders)
                .HasForeignKey(bc => bc.LicenseeId);
        }
    }
}
