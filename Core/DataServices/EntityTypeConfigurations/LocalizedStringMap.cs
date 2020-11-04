using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LocalizedStringMap : IEntityTypeConfiguration<LocalizedString>
    {
        void IEntityTypeConfiguration<LocalizedString>.Configure(EntityTypeBuilder<LocalizedString> builder)
        {
            builder.ToTable("LocalizedStrings", "dbo");
            builder.HasKey(ls => new { ls.Key, ls.CultureCode, ls.Context });

            builder.Property(ls => ls.Key)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(ls => ls.Context)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(ls => ls.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder.Property(ls => ls.Translation)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired();
        }
    }
}
