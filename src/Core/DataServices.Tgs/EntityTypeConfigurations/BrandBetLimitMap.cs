using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandBetLimitMap : IEntityTypeConfiguration<BrandBetLimit>
    {
        void IEntityTypeConfiguration<BrandBetLimit>.Configure(EntityTypeBuilder<BrandBetLimit> builder)
        {
            builder.ToTable("brand_bet_limits");

            builder.HasKey(e => new {e.BrandId, e.BetLimitId})
                .HasName("pk_brand_bet_limits");

            builder.Property(e => e.IsDefault)
                .IsRequired();

            builder
                .HasOne(e => e.Brand)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.BrandId)
                .HasConstraintName("fk_brand_bet_limits_brands_brand_id");

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.BrandBetLimits)
                .HasForeignKey(e => e.BetLimitId)
                .HasConstraintName("fk_brand_bet_limits_bet_limits_bet_limit_id");

            builder.HasIndex(e => e.BetLimitId)
                .HasDatabaseName("ix_brand_bet_limits_bet_limit_id");
        }
    }
}