using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGameMap : IEntityTypeConfiguration<BrandGame>
    {
        void IEntityTypeConfiguration<BrandGame>.Configure(EntityTypeBuilder<BrandGame> builder)
        {
            builder.ToTable("BrandGames", "dbo");

            builder.HasKey(bg => bg.Id);
            builder.HasAlternateKey(bg => new {bg.BrandId, bg.GameId});

            builder.Property(bg => bg.Status)
                .IsRequired();

            builder.Property(bg => bg.Tag)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.EffectiveBettingPercentage)
                .HasColumnType(ColumnTypes.Decimal(18, 6));

            builder
                .HasOne(bg => bg.Brand)
                .WithMany(b => b.Games)
                .HasForeignKey(bg => bg.BrandId);

            builder
                .HasOne(bg => bg.Game)
                .WithMany(g => g.Brands)
                .HasForeignKey(bg => bg.GameId);
        }
    }
}