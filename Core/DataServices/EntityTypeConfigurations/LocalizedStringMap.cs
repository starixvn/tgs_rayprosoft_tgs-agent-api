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
            builder.HasKey(bc => new { bc.Key, bc.Context, bc.CultureCode });

            builder.Property(c => c.Key)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(c => c.Context)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(c => c.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();

            builder.Property(c => c.Translation)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired();
        }
    }
}
