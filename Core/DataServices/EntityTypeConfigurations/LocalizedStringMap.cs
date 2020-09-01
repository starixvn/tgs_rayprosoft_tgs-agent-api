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
            builder.HasKey(bc => new { bc.Context, bc.CultureCode, bc.Code });

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(200))
                .IsRequired();

            builder.Property(c => c.Context)
                .HasColumnType(ColumnTypes.Nvarchar(200))
                .IsRequired();

            builder.Property(c => c.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(10))
                .IsRequired();

            builder.Property(c => c.Translation)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired();
        }
    }
}
