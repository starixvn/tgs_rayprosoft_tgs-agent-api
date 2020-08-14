using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class LocalizedStringTypeConfiguration : IEntityTypeConfiguration<LocalizedString>
    {
        void IEntityTypeConfiguration<LocalizedString>.Configure(EntityTypeBuilder<LocalizedString> builder)
        {
            builder.ToTable("LocalizedStrings", "dbo");
            builder.HasKey(bc => new { bc.Context, bc.CultureCode, bc.Code });

            builder.Property(c => c.Code)
                .HasColumnType("nvarchar(200)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Context)
                .HasColumnType("nvarchar(200)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CultureCode)
                .HasColumnType("nvarchar(200)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Translation)
                .HasColumnType("nvarchar(1024)")
                .IsRequired()
                .IsUnicode();
        }
    }
}
