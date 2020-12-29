using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandBetLimitMap : IEntityTypeConfiguration<BrandBetLimit>
    {
        void IEntityTypeConfiguration<BrandBetLimit>.Configure(EntityTypeBuilder<BrandBetLimit> builder)
        {
            builder.ToTable("BrandBetLimits", "dbo");
            builder.HasKey(e => new {e.BrandId, e.BetLimitId})
                .IsClustered();

            builder.Property(e => e.IsDefault)
                .IsRequired();

            builder
                .HasOne(e => e.Brand)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.BrandId);

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.BrandBetLimits)
                .HasForeignKey(e => e.BetLimitId);
        }
    }
}