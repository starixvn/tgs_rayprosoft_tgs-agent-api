using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class BrandTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<Brand>
    {
        void IEntityTypeConfiguration<Brand>.Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands", "dbo");
            BaseConfigureProperties(builder);

            builder.HasOne(p => p.Licensee)
                .WithMany(p => p.Brands)
                .HasForeignKey(p => p.LicenseeId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Brands)
                .HasForeignKey(p => p.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
