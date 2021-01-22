using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LocalizedStringMap : IEntityTypeConfiguration<LocalizedString>
    {
        void IEntityTypeConfiguration<LocalizedString>.Configure(EntityTypeBuilder<LocalizedString> builder)
        {
            builder.ToTable("localized_strings");

            builder.HasKey(e => new {e.Key, e.CultureCode, e.Context})
                .HasName("pk_localized_strings");

            builder.Property(e => e.Key)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(e => e.Context)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(e => e.CultureCode)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(e => e.Translation)
                .HasMaxLength(1024)
                .IsRequired();

            builder.HasOne(e => e.Language)
                .WithMany(e => e.LocalizedStrings)
                .HasForeignKey(e => e.CultureCode)
                .HasConstraintName("fk_localized_strings_languages_culture_code");

            builder.HasIndex(e => e.CultureCode)
                .HasDatabaseName("ix_localized_strings_culture_code");
        }
    }
}