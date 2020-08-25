using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class MenuTypeConfiguration : IEntityTypeConfiguration<Menu>
    {
        void IEntityTypeConfiguration<Menu>.Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Name)
              .HasColumnType(ColumnTypes.Nvarchar(50))
              .IsRequired();

            builder.HasMany(p => p.RoleMenus)
                .WithOne(p => p.Menu)
                .HasForeignKey(p => p.MenuId);

            builder.Property(c => c.Translation)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired();

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
