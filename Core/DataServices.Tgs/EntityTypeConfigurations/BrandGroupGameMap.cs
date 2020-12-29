using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGroupGameMap : IEntityTypeConfiguration<BrandGroupGame>
    {
        void IEntityTypeConfiguration<BrandGroupGame>.Configure(EntityTypeBuilder<BrandGroupGame> builder)
        {
            builder.ToTable("BrandGroupGames", "dbo");
            builder.HasKey(e => new {e.BrandGroupId, e.BrandGameId});

            builder.HasOne(e => e.BrandGroup)
                .WithMany(e => e.BrandGames)
                .HasForeignKey(e => e.BrandGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.BrandGame)
                .WithMany(e => e.BrandGroups)
                .HasForeignKey(e => e.BrandGameId)
                .OnDelete(DeleteBehavior.Restrict);
            ;
        }
    }
}
