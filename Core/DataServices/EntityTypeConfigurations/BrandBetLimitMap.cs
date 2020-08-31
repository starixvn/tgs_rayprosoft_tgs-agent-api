using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandBetLimitMap : IEntityTypeConfiguration<BrandBetLimit>
    {
        void IEntityTypeConfiguration<BrandBetLimit>.Configure(EntityTypeBuilder<BrandBetLimit> builder)
        {
            builder.ToTable("BrandBetLimits", "dbo");
            builder.HasKey(bc => new { bc.BrandId, bc.BetLimitGroupId });

            builder
                .HasOne(bc => bc.Brand)
                .WithMany(c => c.BetLimitGroups)
                .HasForeignKey(bc => bc.BrandId);

            builder
                .HasOne(bc => bc.BetLimitGroup)
                .WithMany(c => c.Brands)
                .HasForeignKey(bc => bc.BetLimitGroupId);
        }
    }
}
