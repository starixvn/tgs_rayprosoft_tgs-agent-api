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
            builder.HasKey(e => new {e.ResellerId, BetLimitRangeId = e.BetLimitId});

            builder
                .HasOne(e => e.Reseller)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.ResellerId);

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.ResellerBetLimits)
                .HasForeignKey(e => e.BetLimitId);
        }
    }
}