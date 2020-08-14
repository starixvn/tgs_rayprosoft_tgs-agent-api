using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class ResellerTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<Reseller>
    {
        void IEntityTypeConfiguration<Reseller>.Configure(EntityTypeBuilder<Reseller> builder)
        {
            builder.ToTable("Resellers", "dbo");
            BaseConfigureProperties(builder);
            builder.HasMany(c => c.Brands)
                .WithOne(e => e.Reseller);

            builder.HasMany(c => c.Licensees)
                .WithOne(e => e.Reseller);

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)")
                .IsUnicode();

        }
    }
}
