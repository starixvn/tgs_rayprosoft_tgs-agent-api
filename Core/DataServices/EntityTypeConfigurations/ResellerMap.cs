using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class ResellerMap : BaseConfiguration, IEntityTypeConfiguration<Reseller>
    {
        void IEntityTypeConfiguration<Reseller>.Configure(EntityTypeBuilder<Reseller> builder)
        {
            builder.ToTable("Resellers", "dbo");
            BaseConfigureProperties(builder);
            
            builder.HasMany(c => c.Licensees)
                .WithOne(e => e.Reseller);

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.OperationalEmail)
                .HasColumnType(ColumnTypes.Nvarchar(128));
        }
    }
}
