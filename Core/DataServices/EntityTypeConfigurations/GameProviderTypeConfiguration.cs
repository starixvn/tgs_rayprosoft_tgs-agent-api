using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class GameProviderTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<GameProvider>
    {
        void IEntityTypeConfiguration<GameProvider>.Configure(EntityTypeBuilder<GameProvider> builder)
        {
            builder.ToTable("GameProviders", "dbo");
            BaseConfigureProperties(builder);

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(128)");

            builder.Property(c => c.Status)
                .HasColumnType("smallint");

            builder.Property(c => c.PlaceBetTimeout)
                .HasColumnType("int");

            builder.Property(c => c.SettleBetTimeout)
                .HasColumnType("int");

            builder.HasMany(p => p.Games)
                .WithOne(p => p.GameProvider)
                .HasForeignKey(p => p.GameProviderId);
        }
    }
}
