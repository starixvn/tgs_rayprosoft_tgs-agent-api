using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerSportsBetLimitMap : IEntityTypeConfiguration<PlayerSportsBetLimit>
    {
        void IEntityTypeConfiguration<PlayerSportsBetLimit>.Configure(EntityTypeBuilder<PlayerSportsBetLimit> builder)
        {
            builder.ToTable("PlayerSportsBetLimits", "dbo");

            builder.HasKey(e => new {e.PlayerCode, e.SportMarketTypeId})
                .IsClustered();

            builder.Property(e => e.MinBet)
                .IsRequired();

            builder.Property(e => e.MaxBet)
                .IsRequired();

            builder.Property(e => e.MaxBetPerMatch)
                .IsRequired();

            builder.HasOne(e => e.Player)
                .WithMany(e => e.SportsBetLimits)
                .HasForeignKey(e => e.PlayerCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SportMarketType)
                .WithMany(e => e.PlayerSportsBetLimits)
                .HasForeignKey(e => e.SportMarketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
