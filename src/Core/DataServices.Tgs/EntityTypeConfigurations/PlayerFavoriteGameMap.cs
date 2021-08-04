using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerFavoriteGameMap : IEntityTypeConfiguration<PlayerFavoriteGame>
    {
        public void Configure(EntityTypeBuilder<PlayerFavoriteGame> builder)
        {
            builder.ToTable("player_favorite_games");

            builder.HasKey(fg => fg.PlayerCode)
                .HasName("pk_player_favorite_games");

            builder.Property(fg => fg.BrandId)
                .IsRequired();

            builder.Property(fg => fg.GameId)
                .IsRequired();

            builder.Property(fg => fg.GameType)
                .IsRequired();

            builder.Property(fg => fg.SubGameProviderCode)
                .IsRequired();

            builder.HasOne<Brand>()
                .WithOne()
                .HasForeignKey<PlayerFavoriteGame>(fg => fg.BrandId)
                .HasConstraintName("fk_player_favorite_games_brand_brand_id")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Player>()
                .WithOne()
                .HasForeignKey<PlayerFavoriteGame>(fg => fg.PlayerCode)
                .HasConstraintName("fk_player_favorite_games_player_player_code")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
