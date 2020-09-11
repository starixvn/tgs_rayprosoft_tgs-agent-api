using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class RoleMap : IEntityTypeConfiguration<Role>
    {
        void IEntityTypeConfiguration<Role>.Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Name)
              .HasColumnType(ColumnTypes.Nvarchar(50))
              .IsRequired();

            builder.Property(c => c.DisplayName)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired(false);

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired(false);

            builder.Property(c => c.MenuSetting)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired();

            builder.HasMany(p => p.Users)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId);

            builder.Property(c => c.RoleType)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

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
