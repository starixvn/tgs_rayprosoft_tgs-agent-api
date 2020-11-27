using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class ResellerBetLimitMap : IEntityTypeConfiguration<ResellerBetLimit>
    {
        void IEntityTypeConfiguration<ResellerBetLimit>.Configure(EntityTypeBuilder<ResellerBetLimit> builder)
        {
            builder.ToTable("ResellerBetLimits", "dbo");
            builder.HasKey(rbl => new { rbl.ResellerId, rbl.BetLimitGroupId });

            builder
                .HasOne(rbl => rbl.Reseller)
                .WithMany(r => r.BetLimitGroups)
                .HasForeignKey(rbl => rbl.ResellerId);

            builder
                .HasOne(rbl => rbl.BetLimitGroup)
                .WithMany(blg => blg.Resellers)
                .HasForeignKey(rbl => rbl.BetLimitGroupId);
        }
    }
}
