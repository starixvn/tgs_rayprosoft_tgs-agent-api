using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataService.Models;

namespace TGS.Core.DataService.EntityTypeConfiguration
{
    public class GpcTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<GameProviderConfiguration>
    {
        void IEntityTypeConfiguration<GameProviderConfiguration>.Configure(EntityTypeBuilder<GameProviderConfiguration> builder)
        {
            builder.ToTable("GameProviderConfigurations", "dbo");
            BaseConfigureProperties(builder);

        }
    }
}
