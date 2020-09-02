using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class CurrencyMap : IEntityTypeConfiguration<Currency>
    {
        void IEntityTypeConfiguration<Currency>.Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies", "dbo");
            builder.HasKey(p => p.Code);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(c => c.NativeName)
                .HasColumnType(ColumnTypes.Nvarchar(64));

            builder.Property(c => c.Symbol)
                .HasColumnType(ColumnTypes.Nvarchar(16));

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();
        }
    }
}
