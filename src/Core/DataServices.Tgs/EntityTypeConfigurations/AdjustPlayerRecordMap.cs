using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class AdjustPlayerRecordMap : IEntityTypeConfiguration<AdjustPlayerRecord>
    {
        void IEntityTypeConfiguration<AdjustPlayerRecord>.Configure(EntityTypeBuilder<AdjustPlayerRecord> builder)
        {
            builder.ToTable("adjust_player_records");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_adjust_player_record_id");

            builder.Property(c => c.TransactionId);

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.AdjustmentType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.TransferType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.TransferTarget);

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.BucketAmount)
                .HasPrecision(18, 6);

            builder.Property(p => p.Remark);

            builder.Property(l => l.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => new
                {
                    c.CreatedOn,
                    c.BrandId,
                    c.PlayerCode,
                    c.Type
                })
                .HasDatabaseName("ix_adjust_player_record_multiple_keys");
        }
    }
}