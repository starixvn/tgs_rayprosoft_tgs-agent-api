using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class RoleMenuPermissionTypeConfiguration : IEntityTypeConfiguration<RoleMenuPermission>
    {
        void IEntityTypeConfiguration<RoleMenuPermission>.Configure(EntityTypeBuilder<RoleMenuPermission> builder)
        {
            builder.ToTable("RoleMenuPermissions", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Selected)
                .IsRequired();

            builder.Property(c => c.DisplayOrder)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();
        }
    }
}
