using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    internal class SettingMap : IEntityTypeConfiguration<Setting>
    {
        void IEntityTypeConfiguration<Setting>.Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings", "dbo");
            builder.HasKey(c => c.Key);

            builder.Property(c => c.Key)
                .HasColumnType("nvarchar(32)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Value)
                .HasColumnType("nvarchar(4000)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)")
                .IsUnicode();

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
