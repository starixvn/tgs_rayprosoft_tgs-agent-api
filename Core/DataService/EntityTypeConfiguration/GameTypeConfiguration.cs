using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataService.Models;

namespace TGS.Core.DataService.EntityTypeConfiguration
{
    public class GameTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<Game>
    {
        void IEntityTypeConfiguration<Game>.Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games", "dbo");
            BaseConfigureProperties(builder);

        }
    }
}
