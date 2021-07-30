using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    class PlayerGameSettingMap : IEntityTypeConfiguration<PlayerGameSetting>
    {
        public void Configure(EntityTypeBuilder<PlayerGameSetting> builder)
        {
            builder.ToTable("player_game_settings");

            builder.HasKey(pgs => pgs.PlayerCode)
                .HasName("pk_player_game_settings");

            builder.Property(pgs => pgs.IsAutoTransferToGame)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(pgs => pgs.Player)
                .WithOne(p => p.GameSetting)
                .HasForeignKey<PlayerGameSetting>(pgs => pgs.PlayerCode)
                .HasConstraintName("fk_player_game_settings_players");
        }
    }
}
