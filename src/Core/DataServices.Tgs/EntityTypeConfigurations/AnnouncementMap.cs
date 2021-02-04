using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AnnouncementMap : IEntityTypeConfiguration<Announcement>
    {
        void IEntityTypeConfiguration<Announcement>.Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.ToTable("announcements");

            builder.HasKey(c => c.Id)
                .HasName("pk_announcements");

            builder.Property(c => c.Subject)
                .HasMaxLength(256);

            builder.Property(c => c.Content)
                .HasMaxLength(4000);

            builder.Property(c => c.Remarks)
                .HasMaxLength(512);

            builder.Property(c => c.DisplayOrder)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.Property(c => c.From)
                .IsRequired();

            builder.Property(c => c.To)
                .IsRequired();

            builder.Property(c => c.VerifiedByUsername)
                .HasMaxLength(32);

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
                .WithMany(c => c.Announcements)
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_announcements_brands_brand_id");

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_announcements_brand_id");
        }
    }
}