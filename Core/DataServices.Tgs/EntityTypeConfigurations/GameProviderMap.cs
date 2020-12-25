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
            builder.ToTable("GameProviders", "dbo");
            BaseConfigureProperties<GameProvider, Guid>(builder, true);

            builder.Property(p => p.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(p => p.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(p => p.PlaceBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.SettleBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.AutoCloseRound)
                .IsRequired();

            builder.Property(p => p.AllowKickPlayer)
                .IsRequired();

            builder.HasMany(p => p.Brands)
                .WithOne(lgpc => lgpc.GameProvider)
                .HasForeignKey(lgpc => lgpc.GameProviderId);
        }
    }
}
