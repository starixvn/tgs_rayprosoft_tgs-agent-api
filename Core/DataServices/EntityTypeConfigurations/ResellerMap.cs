using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class ResellerMap : BaseConfiguration, IEntityTypeConfiguration<Reseller>
    {
        void IEntityTypeConfiguration<Reseller>.Configure(EntityTypeBuilder<Reseller> builder)
        {
            builder.ToTable("Resellers", "dbo");
            BaseConfigureProperties(builder);
            
            builder.HasMany(c => c.Brands)
                .WithOne(e => e.Reseller);

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)")
                .IsUnicode();

            builder.Property(c => c.OperationalEmail)
                .HasColumnType("nvarchar(128)")
                .IsUnicode();
        }
    }
}
