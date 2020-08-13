using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
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
