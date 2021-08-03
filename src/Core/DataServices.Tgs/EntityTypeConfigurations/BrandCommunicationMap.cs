using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandCommunicationMap : IEntityTypeConfiguration<BrandCommunication>
    {
        void IEntityTypeConfiguration<BrandCommunication>.Configure(EntityTypeBuilder<BrandCommunication> builder)
        {
            builder.ToTable("brand_communications");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Brand)
                .WithMany()
                .HasForeignKey(e => e.BrandId)
                .HasConstraintName("fk_brand_communications_brands_brand_id");

            builder.Property(e => e.Type)
                .HasMaxLength(64);

            builder.Property(e => e.Phone)
                .HasMaxLength(32);

            builder.Property(e => e.AppId)
                .HasMaxLength(128);

            builder.Property(e => e.QRCodeUrl)
                .HasMaxLength(1024);
        }
    }
}