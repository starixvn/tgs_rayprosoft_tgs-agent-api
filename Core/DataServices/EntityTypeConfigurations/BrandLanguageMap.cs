using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandLanguageMap : IEntityTypeConfiguration<BrandLanguage>
    {
        void IEntityTypeConfiguration<BrandLanguage>.Configure(EntityTypeBuilder<BrandLanguage> builder)
        {
            builder.ToTable("BrandLanguages", "dbo");

            builder.HasKey(bl => new {bl.BrandId, bl.CultureCode});

            builder
                .HasOne(bl => bl.Brand)
                .WithMany(b => b.BrandLanguages)
                .HasForeignKey(bl => bl.BrandId);

            builder
                .HasOne(bl => bl.Language)
                .WithMany(l => l.BrandLanguages)
                .HasForeignKey(bl => bl.CultureCode);

            builder.Property(bl => bl.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsUnicode();

            builder.Property(bl => bl.BrandLanguageCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();
        }
    }
}
