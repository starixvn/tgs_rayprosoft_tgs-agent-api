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
            builder.HasKey(e => new { e.LicenseeId, BetLimitRangeId = e.BetLimitId });

            builder
                .HasOne(e => e.Licensee)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.LicenseeId);

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.LicenseeBetLimits)
                .HasForeignKey(e => e.BetLimitId);
        }
    }
}
