using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LanguageMap : IEntityTypeConfiguration<Language>
    {
        void IEntityTypeConfiguration<Language>.Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("languages");

            builder.HasKey(l => l.CultureCode)
                .HasName("pk_languages");

            builder.Property(l => l.CultureCode)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(l => l.LanguageCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(l => l.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(l => l.LocalizedName)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(l => l.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.CreatedOn)
                .IsRequired();

            builder.Property(l => l.UpdatedOn)
                .IsRequired();
        }
    }
}