using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderMap : BaseConfiguration, IEntityTypeConfiguration<GameProvider>
    {
        void IEntityTypeConfiguration<GameProvider>.Configure(EntityTypeBuilder<GameProvider> builder)
        {
            builder.ToTable("game_providers");

            ConfigureBaseProperties<GameProvider, Guid>(builder, "pk_game_providers");

            builder.Property(e => e.Description)
                .HasMaxLength(128);

            builder.Property(e => e.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(e => e.PlaceBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.SettleBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.AutoCloseRound)
                .IsRequired();

            builder.Property(e => e.AllowKickPlayer)
                .IsRequired();
        }
    }
}