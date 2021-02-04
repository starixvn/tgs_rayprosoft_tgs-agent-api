using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandLanguageMap : IEntityTypeConfiguration<BrandLanguage>
    {
        void IEntityTypeConfiguration<BrandLanguage>.Configure(EntityTypeBuilder<BrandLanguage> builder)
        {
            builder.ToTable("brand_languages");

            builder.HasKey(bl => new {bl.BrandId, bl.CultureCode})
                .HasName("pk_brand_languages");

            builder.Property(bl => bl.CultureCode)
                .HasMaxLength(6)
                .IsUnicode();

            builder.Property(bl => bl.BrandCultureCode)
                .HasMaxLength(6)
                .IsRequired();

            builder
                .HasOne(bl => bl.Brand)
                .WithMany(b => b.Languages)
                .HasForeignKey(bl => bl.BrandId)
                .HasConstraintName("fk_brand_languages_brands_brand_id");

            builder
                .HasOne(bl => bl.Language)
                .WithMany(l => l.BrandLanguages)
                .HasForeignKey(bl => bl.CultureCode)
                .HasConstraintName("fk_brand_languages_languages_culture_code");

            builder.HasIndex(e => e.CultureCode)
                .HasDatabaseName("ix_brand_languages_culture_code");
        }
    }
}