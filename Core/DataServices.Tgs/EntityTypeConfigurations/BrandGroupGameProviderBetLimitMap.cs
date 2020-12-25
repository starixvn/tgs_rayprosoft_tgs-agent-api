using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BrandGroupGameProviderBetLimitMap : IEntityTypeConfiguration<BrandGroupGameProviderBetLimit>
    {
        void IEntityTypeConfiguration<BrandGroupGameProviderBetLimit>.Configure(
            EntityTypeBuilder<BrandGroupGameProviderBetLimit> builder)
        {
            builder.ToTable("BrandGroupGameProviderBetLimits", "dbo");
            builder.HasKey(e => new {e.BrandGroupBetLimitId, e.GameProviderBetLimitId});

            builder.HasOne(e => e.BrandGroupBetLimit)
                .WithMany(e => e.GameProviderBetLimits)
                .HasForeignKey(e => e.BrandGroupBetLimitId);

            builder.HasOne(e => e.GameProviderBetLimit)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.GameProviderBetLimitId);
        }
    }
}