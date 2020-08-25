using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public sealed class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Username)
              .HasColumnType(ColumnTypes.Nvarchar(50))
              .IsRequired();

            builder.Property(c => c.RoleId)
                .IsRequired();

            builder.HasOne(p => p.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(p => p.RoleId);

            builder.Property(c => c.RoleType)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.ResellerId)
                .IsRequired(false);

            builder.Property(c => c.LicenseeId)
                .IsRequired(false);

            builder.Property(c => c.BrandId)
                .IsRequired(false);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Firstname)
                .HasColumnType(ColumnTypes.Nvarchar(30))
                .IsRequired();

            builder.Property(c => c.Lastname)
                .HasColumnType(ColumnTypes.Nvarchar(30))
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(c => c.TimeZone)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired();

            builder.Property(c => c.Password)
                .HasColumnType(ColumnTypes.Varbinary(128))
                .IsRequired();

            builder.Property(c => c.Salt)
                .HasColumnType(ColumnTypes.Varbinary(128))
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
