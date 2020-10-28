using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LanguageMap : IEntityTypeConfiguration<Language>
    {
        void IEntityTypeConfiguration<Language>.Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "dbo");
            builder.HasKey(l => l.CultureCode);

            builder.Property(l => l.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.LanguageCode)
                .HasColumnType(ColumnTypes.Nvarchar(10))
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.Name)
                .HasColumnType(ColumnTypes.Nvarchar(30))
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired()
                .IsUnicode();

            builder.Property(l => l.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(l => l.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(l => l.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.HasMany(l => l.Players)
                .WithOne(p => p.Language)
                .HasForeignKey(p => p.CultureCode);

            builder.HasMany(l => l.LicenseeLanguages)
                .WithOne(bl => bl.Language)
                .HasForeignKey(bl => bl.CultureCode);

            builder.HasMany(l => l.GameProviderLanguages)
                .WithOne(gpl => gpl.Language)
                .HasForeignKey(gpl => gpl.CultureCode);

            builder.HasMany(l => l.LicenseeGameProviderConfigurations)
                .WithOne(bgpc => bgpc.DefaultLanguage)
                .HasForeignKey(bgpc => bgpc.DefaultLanguageCode);

            builder.HasMany(l => l.LocalizedStrings)
                .WithOne(ls => ls.Language)
                .HasForeignKey(ls => ls.CultureCode);
        }
    }
}