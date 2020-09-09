using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeGameMap : IEntityTypeConfiguration<LicenseeGame>
    {
        void IEntityTypeConfiguration<LicenseeGame>.Configure(EntityTypeBuilder<LicenseeGame> builder)
        {
            builder.ToTable("LicenseeGames", "dbo");
            builder.HasKey(bc => new { bc.LicenseeId, bc.GameId });
            builder
                .HasOne(bc => bc.Licensee)
                .WithMany(c => c.Games)
                .HasForeignKey(bc => bc.LicenseeId);
            builder
                .HasOne(bc => bc.Game)
                .WithMany(c => c.Licensees)
                .HasForeignKey(bc => bc.GameId);

        }
    }
}
