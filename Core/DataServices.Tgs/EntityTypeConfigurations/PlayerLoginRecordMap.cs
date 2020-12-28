using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerLoginRecordMap : IEntityTypeConfiguration<PlayerLoginRecord>
    {
        void IEntityTypeConfiguration<PlayerLoginRecord>.Configure(EntityTypeBuilder<PlayerLoginRecord> builder)
        {
            builder.ToTable("PlayerLoginRecords");
            builder.HasKey(b => b.Id)
                .IsClustered(false);

            builder.Property(b => b.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(b => b.BrandId)
                .IsRequired();

            builder.Property(b => b.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.PlatformType)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(b => b.ExpiredOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.HasOne(b => b.Player)
                .WithMany(b => b.PlayerLoginRecords)
                .HasForeignKey(b => b.PlayerCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Brand)
                .WithMany(b => b.PlayerLoginRecords)
                .HasForeignKey(b => b.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
