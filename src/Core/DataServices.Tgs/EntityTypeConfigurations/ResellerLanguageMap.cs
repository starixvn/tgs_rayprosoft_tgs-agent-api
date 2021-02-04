using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ResellerLanguageMap : IEntityTypeConfiguration<ResellerLanguage>
    {
        void IEntityTypeConfiguration<ResellerLanguage>.Configure(EntityTypeBuilder<ResellerLanguage> builder)
        {
            builder.ToTable("reseller_languages");

            builder.HasKey(e => new {e.ResellerId, e.CultureCode})
                .HasName("pk_reseller_languages");

            builder
                .HasOne(e => e.Reseller)
                .WithMany(e => e.Languages)
                .HasForeignKey(e => e.ResellerId)
                .HasConstraintName("fk_reseller_languages_resellers_reseller_id");

            builder
                .HasOne(e => e.Language)
                .WithMany(e => e.ResellerLanguages)
                .HasForeignKey(e => e.CultureCode)
                .HasConstraintName("fk_reseller_languages_languages_culture_code");

            builder.HasIndex(e => e.CultureCode)
                .HasDatabaseName("ix_reseller_languages_culture_code");
        }
    }
}