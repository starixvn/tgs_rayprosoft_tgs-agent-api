using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGroupBetLimitMap : IEntityTypeConfiguration<BrandGroupBetLimit>
    {
        void IEntityTypeConfiguration<BrandGroupBetLimit>.Configure(EntityTypeBuilder<BrandGroupBetLimit> builder)
        {
            builder.ToTable("brand_group_bet_limits");

            builder.HasKey(e => e.Id)
                .HasName("pk_brand_group_bet_limits");

            builder.HasAlternateKey(e => new {e.BrandGroupId, e.BetLimitId})
                .HasName("ak_brand_group_bet_limits_brand_group_id_bet_limit_id");

            builder
                .HasOne(e => e.BrandGroup)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.BrandGroupId)
                .HasConstraintName("fk_brand_group_bet_limits_brand_groups_brand_group_id");

            builder.HasIndex(e => e.BrandGroupId)
                .HasDatabaseName("ix_brand_group_bet_limits_brand_group_id");

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.BetLimitId)
                .HasConstraintName("fk_brand_group_bet_limits_bet_limits_bet_limit_id");

            builder.HasIndex(e => e.BetLimitId)
                .HasDatabaseName("ix_brand_group_bet_limits_bet_limit_id");
        }
    }
}