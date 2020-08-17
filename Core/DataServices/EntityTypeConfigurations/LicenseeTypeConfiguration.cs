using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class LicenseeTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<Licensee>
    {
        void IEntityTypeConfiguration<Licensee>.Configure(EntityTypeBuilder<Licensee> builder)
        {
            builder.ToTable("Licensees", "dbo");
            BaseConfigureProperties(builder);

            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Licensees)
                .HasForeignKey(p => p.ResellerId);

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(128)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Status)
                .HasColumnType("smallint")
                .IsRequired();

        }
    }
}
