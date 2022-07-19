using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class GamePlayerTransformIdnLiveMap : BaseConfiguration, IEntityTypeConfiguration<GamePlayerTransformIdnLive>
{
    void IEntityTypeConfiguration<GamePlayerTransformIdnLive>.Configure(EntityTypeBuilder<GamePlayerTransformIdnLive> builder)
    {
        builder.ToTable("game_player_transform_idnlive");

        builder.HasIndex(e => e.PlayerCode, "ix_game_player_transform_idnlive_player_code")
            .IsUnique();

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasIdentityOptions(10000001L, null, null, null, null, null);

        builder.Property(e => e.Configuration)
            .IsRequired()
            .HasColumnType("jsonb")
            .HasColumnName("configuration");

        builder.Property(e => e.CreatedOn)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_on");

        builder.Property(e => e.PlayerCode)
            .IsRequired()
            .HasColumnType("character varying")
            .HasColumnName("player_code");

        builder.Property(e => e.UpdatedOn)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("updated_on");
    }
}