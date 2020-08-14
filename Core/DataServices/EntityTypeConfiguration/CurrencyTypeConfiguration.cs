using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class CurrencyTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        void IEntityTypeConfiguration<Currency>.Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currecies", "dbo");
            builder.HasKey(p => p.Code);

            builder.Property(c => c.NativeName)
                .HasColumnType("nvarchar(64)");

            builder.Property(c => c.Symbol)
                .HasColumnType("nvarchar(16)");

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
