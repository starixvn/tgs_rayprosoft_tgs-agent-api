using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class GameProviderTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<GameProvider>
    {
        void IEntityTypeConfiguration<GameProvider>.Configure(EntityTypeBuilder<GameProvider> builder)
        {
            builder.ToTable("GameProviders", "dbo");
            BaseConfigureProperties(builder);

        }
    }
}
