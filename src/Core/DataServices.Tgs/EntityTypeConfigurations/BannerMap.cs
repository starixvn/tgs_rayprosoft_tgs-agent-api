using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BannerMap : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("banners")
                .HasKey(b => b.Id);

            builder.Property(b => b.BrandId)
                .IsRequired();

            builder.Property(b => b.BeginTime)
                .IsRequired();
            builder.Property(b => b.EndTime);

            builder.Property(b => b.IsEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.Title)
                .IsRequired();
            builder.Property(b => b.Status)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(b => b.Localization)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(b => b.CreatedByUserName)
                .IsRequired();
            builder.Property(b => b.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");
            builder.Property(b => b.UpdatedByUserName)
                .IsRequired();
            builder.Property(b => b.UpdatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(b => b.BrandId)
                .HasConstraintName("fk_banners_brands_brand_id")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
