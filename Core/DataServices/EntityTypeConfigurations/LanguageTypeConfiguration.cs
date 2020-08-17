using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class LanguageTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        void IEntityTypeConfiguration<Language>.Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "dbo");
            builder.HasKey(p => p.CultureCode);

            builder.Property(c => c.CultureCode)
                .HasColumnType("nvarchar(6)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.LanguageCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Name)
                .HasColumnType("nvarchar(30)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}
