using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class LicenseeLanguageTypeConfiguration : IEntityTypeConfiguration<LicenseeLanguage>
    {
        void IEntityTypeConfiguration<LicenseeLanguage>.Configure(EntityTypeBuilder<LicenseeLanguage> builder)
        {
            builder.ToTable("LicenseeLanguages", "dbo");
            builder.HasKey(bc => new { bc.LicenseeId, bc.CultureCode });
            builder
                .HasOne(bc => bc.Licensee)
                .WithMany(c => c.Languages)
                .HasForeignKey(bc => bc.LicenseeId);
            builder
                .HasOne(bc => bc.Language)
                .WithMany(c => c.LicenseeLanguages)
                .HasForeignKey(bc => bc.CultureCode);
        }
    }
}
