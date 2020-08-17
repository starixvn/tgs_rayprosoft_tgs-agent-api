using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class BrandGameTypeConfiguration : IEntityTypeConfiguration<BrandGame>
    {
        void IEntityTypeConfiguration<BrandGame>.Configure(EntityTypeBuilder<BrandGame> builder)
        {
            builder.ToTable("BrandGames", "dbo");
            builder.HasKey(bc => new { bc.BrandId, bc.GameId });
            builder
                .HasOne(bc => bc.Brand)
                .WithMany(c => c.Games)
                .HasForeignKey(bc => bc.BrandId);
            builder
                .HasOne(bc => bc.Game)
                .WithMany(c => c.Brands)
                .HasForeignKey(bc => bc.GameId);

        }
    }
}
