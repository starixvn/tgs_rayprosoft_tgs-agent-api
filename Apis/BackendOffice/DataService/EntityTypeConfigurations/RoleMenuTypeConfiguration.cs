using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class RoleMenuTypeConfiguration : IEntityTypeConfiguration<RoleMenu>
    {
        void IEntityTypeConfiguration<RoleMenu>.Configure(EntityTypeBuilder<RoleMenu> builder)
        {
            builder.ToTable("RoleMenus", "dbo");

            builder.HasKey(p => p.Id);


            builder.HasMany(p => p.Permissions)
                .WithOne(p => p.RoleMenu)
                .HasForeignKey(p => p.RoleMenuId);

            builder.HasMany(p => p.SubMenus)
                .WithOne(p => p.Parent)
                .HasForeignKey(p => p.ParentId);

            builder.Property(c => c.Selected)
                .IsRequired();
        }
    }
}
