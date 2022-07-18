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
            .WithMany()
            .HasForeignKey(bm => bm.BrandId)
            .HasConstraintName("fk_brand_maintenance_brands_brand_id");

        builder.Property(bm => bm.Content)
            .HasMaxLength(4000);

        builder.Property(bm => bm.BeginTime)
            .IsRequired();

        builder.Property(bm => bm.EndTime)
            .IsRequired();
    }
}
