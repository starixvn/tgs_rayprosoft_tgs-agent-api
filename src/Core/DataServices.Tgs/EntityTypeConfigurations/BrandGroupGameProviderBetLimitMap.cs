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
            builder.ToTable("brand_group_game_provider_bet_limits");

            builder.HasKey(e => new {e.BrandGroupBetLimitId, e.GameProviderBetLimitId})
                .HasName("pk_brand_group_game_provider_bet_limits");

            builder.HasOne(e => e.BrandGroupBetLimit)
                .WithMany(e => e.GameProviderBetLimits)
                .HasForeignKey(e => e.BrandGroupBetLimitId)
                .HasConstraintName(
                    "fk_brand_group_game_provider_bet_limits_brand_group_bet_limits_brand_group_bet_limit_id");

            builder.HasOne(e => e.GameProviderBetLimit)
                .WithMany(e => e.BrandGroupBetLimits)
                .HasForeignKey(e => e.GameProviderBetLimitId)
                .HasConstraintName(
                    "fk_brand_group_game_provider_bet_limits_game_provider_bet_limits_game_provider_bet_limit_id");

            builder.HasIndex(e => e.GameProviderBetLimitId)
                .HasDatabaseName("ix_brand_group_game_provider_bet_limits_game_provider_bet_limit_id");
        }
    }
}