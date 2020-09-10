using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeBetLimitMap : IEntityTypeConfiguration<LicenseeBetLimit>
    {
        void IEntityTypeConfiguration<LicenseeBetLimit>.Configure(EntityTypeBuilder<LicenseeBetLimit> builder)
        {
            builder.ToTable("LicenseeBetLimits", "dbo");
            builder.HasKey(bc => new { bc.LicenseeId, bc.BetLimitGroupId });

            builder
                .HasOne(bc => bc.Licensee)
                .WithMany(c => c.BetLimitGroups)
                .HasForeignKey(bc => bc.LicenseeId);

            builder
                .HasOne(bc => bc.BetLimitGroup)
                .WithMany(c => c.Licensees)
                .HasForeignKey(bc => bc.BetLimitGroupId);
        }
    }
}
