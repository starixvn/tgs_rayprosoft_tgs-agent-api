using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class SettingMap : IEntityTypeConfiguration<Setting>
    {
        void IEntityTypeConfiguration<Setting>.Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings", "dbo");
            builder.HasKey(c => c.Key);

            builder.Property(c => c.Key)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Value)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

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
