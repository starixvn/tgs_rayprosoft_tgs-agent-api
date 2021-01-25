using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandGroupGameMap : IEntityTypeConfiguration<BrandGroupGame>
    {
        void IEntityTypeConfiguration<BrandGroupGame>.Configure(EntityTypeBuilder<BrandGroupGame> builder)
        {
            builder.ToTable("brand_group_games");

            builder.HasKey(e => new {e.BrandGroupId, e.BrandGameId})
                .HasName("pk_brand_group_games");

            builder.HasOne(e => e.BrandGroup)
                .WithMany(e => e.BrandGames)
                .HasForeignKey(e => e.BrandGroupId)
                .HasConstraintName("fk_brand_group_games_brand_groups_brand_group_id");

            builder.HasOne(e => e.BrandGame)
                .WithMany(e => e.BrandGroups)
                .HasForeignKey(e => e.BrandGameId)
                .HasConstraintName("fk_brand_group_games_brand_games_brand_game_id");

            builder.HasIndex(e => e.BrandGameId)
                .HasDatabaseName("ix_brand_group_games_brand_game_id");
        }
    }
}