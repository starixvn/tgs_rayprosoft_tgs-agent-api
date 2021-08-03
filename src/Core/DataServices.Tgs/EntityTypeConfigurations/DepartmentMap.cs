using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        void IEntityTypeConfiguration<Department>.Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments");

            builder.HasKey(p => p.Id)
                .HasName("pk_departments");

            builder.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(p => p.Users)
                .WithOne(p => p.Department)
                .HasForeignKey(p => p.DepartmentId);

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.HasOne(c => c.Brand)
                .WithMany(c => c.Departments)
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_departments_brands_brand_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_departments_brand_id");

            builder.HasOne(c => c.Licensee)
                .WithMany(c => c.Departments)
                .HasForeignKey(c => c.LicenseeId)
                .HasConstraintName("fk_departments_licensees_licensee_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.LicenseeId)
                .HasDatabaseName("ix_departments_licensee_id");

            builder.HasOne(c => c.Reseller)
                .WithMany(c => c.Departments)
                .HasForeignKey(c => c.ResellerId)
                .HasConstraintName("fk_departments_resellers_reseller_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.ResellerId)
                .HasDatabaseName("ix_departments_reseller_id");
        }
    }
}