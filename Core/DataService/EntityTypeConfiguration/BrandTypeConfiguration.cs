using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataService.Models;

namespace TGS.Core.DataService.EntityTypeConfiguration
{
    public class BrandTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<Brand>
    {
        void IEntityTypeConfiguration<Brand>.Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("ApiLogs", "dbo");
            BaseConfigureProperties(builder);

        }
    }
}
