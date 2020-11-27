using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeMap : BaseConfiguration, IEntityTypeConfiguration<Licensee>
    {
        void IEntityTypeConfiguration<Licensee>.Configure(EntityTypeBuilder<Licensee> builder)
        {
            builder.ToTable("Licensees", "dbo");
            BaseConfigureProperties(builder);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.OperationalEmail)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Licensees)
                .HasForeignKey(p => p.ResellerId);
        }
    }
}
