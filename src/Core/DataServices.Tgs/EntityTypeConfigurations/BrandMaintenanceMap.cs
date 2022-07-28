using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class BrandMaintenanceMap : IEntityTypeConfiguration<BrandMaintenance>
{
    void IEntityTypeConfiguration<BrandMaintenance>.Configure(EntityTypeBuilder<BrandMaintenance> builder)
    {
        builder.ToTable("brand_maintenance");

        builder.HasKey(bm => bm.Id);

        builder.HasOne(bm => bm.Brand)
           .WithOne()
           .HasForeignKey<BrandMaintenance>(bm => bm.BrandId)
           .HasConstraintName("fk_brand_maintenance_brands_brand_id");

        builder.Property(bm => bm.Content)            
            .HasMaxLength(2000);

        builder.Property(bm => bm.BeginTime)
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(bm => bm.EndTime)
            .HasColumnType("time without time zone")
            .IsRequired();
    }
}
