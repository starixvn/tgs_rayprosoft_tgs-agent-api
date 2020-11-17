using FileService.DataService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;

namespace FileService.DataService.EntityTypeConfigurations
{
    public sealed class FileMap : IEntityTypeConfiguration<File>
    {
        void IEntityTypeConfiguration<File>.Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable("Files", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.FileName)
                .HasColumnType(ColumnTypes.Nvarchar(100))
                .IsRequired();

            builder.Property(c => c.ContentType)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired(false);

            builder.Property(c => c.FileLength)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired(false);
        }
    }
}