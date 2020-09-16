using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class SettingMap : IEntityTypeConfiguration<Setting>
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
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

        }
    }
}
