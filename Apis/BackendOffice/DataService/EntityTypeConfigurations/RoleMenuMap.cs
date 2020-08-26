using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class RoleMenuMap : IEntityTypeConfiguration<RoleMenu>
    {
        void IEntityTypeConfiguration<RoleMenu>.Configure(EntityTypeBuilder<RoleMenu> builder)
        {
            builder.ToTable("RoleMenus", "dbo");

            builder.HasKey(p => p.Id)
                .IsClustered(false);

            builder.HasIndex(p => new { p.RoleId, p.MenuId })
                .IsUnique()
                .IsClustered();

            builder.HasMany(p => p.Permissions)
                .WithOne(p => p.RoleMenu)
                .HasForeignKey(p => p.RoleMenuId);

            builder.HasMany(p => p.SubMenus)
                .WithOne(p => p.Parent)
                .HasForeignKey(p => p.ParentId);

            builder.Property(c => c.IsDisplay)
                .IsRequired();
        }
    }
}
