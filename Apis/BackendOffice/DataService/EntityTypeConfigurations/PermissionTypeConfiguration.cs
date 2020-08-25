using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class PermissionTypeConfiguration : IEntityTypeConfiguration<Permission>
    {
        void IEntityTypeConfiguration<Permission>.Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Name)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.HasMany(p => p.RoleMenuPermissions)
                .WithOne(p => p.Permission)
                .HasForeignKey(p => p.PermissionId);

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(255))
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
