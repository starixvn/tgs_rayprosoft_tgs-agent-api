using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameProviderMap : BaseConfiguration, IEntityTypeConfiguration<GameProvider>
    {
        void IEntityTypeConfiguration<GameProvider>.Configure(EntityTypeBuilder<GameProvider> builder)
        {
            builder.ToTable("GameProviders", "dbo");
            BaseConfigureProperties(builder, true);

            builder.Property(p => p.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(p => p.Status)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.Property(p => p.PlaceBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.Property(p => p.SettleBetTimeout)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.HasMany(p => p.LicenseeGameProviderConfigurations)
                .WithOne(p => p.GameProvider)
                .HasForeignKey(bgpc => bgpc.GameProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
