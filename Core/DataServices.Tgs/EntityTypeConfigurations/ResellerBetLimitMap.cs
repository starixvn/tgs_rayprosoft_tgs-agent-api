using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ResellerBetLimitMap : IEntityTypeConfiguration<ResellerBetLimit>
    {
        void IEntityTypeConfiguration<ResellerBetLimit>.Configure(EntityTypeBuilder<ResellerBetLimit> builder)
        {
            builder.ToTable("reseller_bet_limits");

            builder.HasKey(e => new {e.ResellerId, e.BetLimitId})
                .HasName("pk_reseller_bet_limits");

            builder
                .HasOne(e => e.Reseller)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.ResellerId)
                .HasConstraintName("fk_reseller_bet_limits_resellers_reseller_id");

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.ResellerBetLimits)
                .HasForeignKey(e => e.BetLimitId)
                .HasConstraintName("fk_reseller_bet_limits_resellers_bet_limit_id");

            builder.HasIndex(e => e.BetLimitId)
                .HasDatabaseName("ix_reseller_bet_limits_bet_limit_id");
        }
    }
}