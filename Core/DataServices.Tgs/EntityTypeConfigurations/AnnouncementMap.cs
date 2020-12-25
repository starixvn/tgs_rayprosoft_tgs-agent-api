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
            builder.ToTable("Announcements", "dbo");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Subject)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.Content)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.Remarks)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.DisplayOrder)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.Property(c => c.From)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.To)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.VerifiedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired(false);

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
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
