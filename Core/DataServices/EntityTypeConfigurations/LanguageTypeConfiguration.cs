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
            builder.HasKey(l => l.CultureCode);

            builder.Property(l => l.CultureCode)
                .HasColumnType("nvarchar(6)")
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.LanguageCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.Name)
                .HasColumnType("nvarchar(30)")
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.CreatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.UpdatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.CreatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(l => l.UpdatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}
