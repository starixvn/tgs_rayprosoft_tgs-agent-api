using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeLanguageMap : IEntityTypeConfiguration<LicenseeLanguage>
    {
        void IEntityTypeConfiguration<LicenseeLanguage>.Configure(EntityTypeBuilder<LicenseeLanguage> builder)
        {
            builder.ToTable("LicenseeLanguages", "dbo");

            builder.HasKey(bl => new {bl.LicenseeId, bl.CultureCode});

            builder
                .HasOne(bl => bl.Licensee)
                .WithMany(b => b.Languages)
                .HasForeignKey(bl => bl.LicenseeId);

            builder
                .HasOne(bl => bl.Language)
                .WithMany(l => l.LicenseeLanguages)
                .HasForeignKey(bl => bl.CultureCode);

            builder.Property(bl => bl.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsUnicode();
        }
    }
}