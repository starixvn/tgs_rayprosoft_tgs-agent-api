using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandAllAgentMap : IEntityTypeConfiguration<BrandAllAgent>
    {
        void IEntityTypeConfiguration<BrandAllAgent>.Configure(EntityTypeBuilder<BrandAllAgent> builder)
        {
            builder.ToTable("brand_all_agents");

            builder.HasKey(bc => bc.BrandId)
                .HasName("pk_brand_all_agents");
        }
    }
}