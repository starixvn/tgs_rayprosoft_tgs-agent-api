using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGroupBetLimitMap : IEntityTypeConfiguration<BrandGroupBetLimit>
    {
        void IEntityTypeConfiguration<BrandGroupBetLimit>.Configure(EntityTypeBuilder<BrandGroupBetLimit> builder)
        {
            builder.ToTable("BrandGroupBetLimits", "dbo");

            builder.HasKey(e => e.Id)
                .IsClustered(false);

            builder.HasAlternateKey(e => new {e.BrandGroupId, e.BetLimitId})
                .IsClustered();

            builder
                .HasOne(e => e.BrandGroup)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.BrandGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.BetLimitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}