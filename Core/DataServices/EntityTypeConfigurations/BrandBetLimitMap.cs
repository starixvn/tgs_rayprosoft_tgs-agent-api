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
                .HasOne(bbl => bbl.Brand)
                .WithMany(b => b.BetLimitGroups)
                .HasForeignKey(bbl => bbl.BrandId);

            builder
                .HasOne(bbl => bbl.BetLimitGroup)
                .WithMany(blg => blg.Brands)
                .HasForeignKey(bbl => bbl.BetLimitGroupId);
        }
    }
}
