using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeBetLimitMap : IEntityTypeConfiguration<LicenseeBetLimit>
    {
        void IEntityTypeConfiguration<LicenseeBetLimit>.Configure(EntityTypeBuilder<LicenseeBetLimit> builder)
        {
            builder.ToTable("licensee_bet_limits");

            builder.HasKey(e => new {e.LicenseeId, e.BetLimitId})
                .HasName("pk_licensee_bet_limits");

            builder
                .HasOne(e => e.Licensee)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.LicenseeId)
                .HasConstraintName("fk_licensee_bet_limits_licensees_licensee_id");

            builder
                .HasOne(e => e.BetLimit)
                .WithMany(e => e.LicenseeBetLimits)
                .HasForeignKey(e => e.BetLimitId)
                .HasConstraintName("fk_licensee_bet_limits_bet_limits_bet_limit_id");

            builder.HasIndex(e => e.BetLimitId)
                .HasDatabaseName("ix_licensee_bet_limits_bet_limit_id");
        }
    }
}