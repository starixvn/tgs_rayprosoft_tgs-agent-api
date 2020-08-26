using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameProviderMap : BaseConfiguration, IEntityTypeConfiguration<GameProvider>
    {
        void IEntityTypeConfiguration<GameProvider>.Configure(EntityTypeBuilder<GameProvider> builder)
        {
            builder.ToTable("GameProviders", "dbo");
            BaseConfigureProperties(builder);

            builder.Property(p => p.Description)
                .HasColumnType("nvarchar(128)");

            builder.Property(p => p.Status)
                .HasColumnType("smallint");

            builder.Property(p => p.PlaceBetTimeout)
                .HasColumnType("int");

            builder.Property(p => p.SettleBetTimeout)
                .HasColumnType("int");

            builder.HasMany(p => p.BrandGameProviderConfigurations)
                .WithOne(p => p.GameProvider)
                .HasForeignKey(bgpc => bgpc.GameProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
