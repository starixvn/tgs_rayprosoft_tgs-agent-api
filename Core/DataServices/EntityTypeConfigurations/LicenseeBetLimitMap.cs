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
                .HasOne(lbl => lbl.Licensee)
                .WithMany(l => l.BetLimitGroups)
                .HasForeignKey(lbl => lbl.LicenseeId);

            builder
                .HasOne(lbl => lbl.BetLimitGroup)
                .WithMany(blg => blg.Licensees)
                .HasForeignKey(lbl => lbl.BetLimitGroupId);
        }
    }
}
