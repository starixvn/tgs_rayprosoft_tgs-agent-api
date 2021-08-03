using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class StationLetterMap : IEntityTypeConfiguration<StationLetter>
    {
        void IEntityTypeConfiguration<StationLetter>.Configure(EntityTypeBuilder<StationLetter> builder)
        {
            builder.ToTable("station_letters");

            builder.HasKey(c => c.Id)
                .HasName("pk_station_letters");

            builder.Property(c => c.Subject)
                .HasMaxLength(256);

            builder.Property(c => c.Content)
                .HasMaxLength(4000);

            builder.Property(c => c.Localization)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired()
                .HasDefaultValue("{}");

            builder.Property(c => c.Remarks)
                .HasMaxLength(512);

            builder.Property(c => c.VerifiedByUsername)
                .HasMaxLength(256);

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

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
                .WithMany(c => c.StationLetters)
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_station_letters_brands_brand_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.BrandId)
                .HasDatabaseName("ix_station_letters_brand_id");
        }
    }
}
