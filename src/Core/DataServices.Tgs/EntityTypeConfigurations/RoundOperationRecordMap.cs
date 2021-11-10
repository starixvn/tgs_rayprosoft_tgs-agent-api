using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class RoundOperationRecordMap : IEntityTypeConfiguration<RoundOperationRecord>
    {
        void IEntityTypeConfiguration<RoundOperationRecord>.Configure(EntityTypeBuilder<RoundOperationRecord> builder)
        {
            builder.ToTable("round_operation_records", Consts.BetSchema);

            builder.HasKey(c => c.Id)
                .HasName("pk_round_operation_records");

            builder.Property(c => c.RoundId)
                .IsRequired();

            builder.Property(c => c.OperatorId)
                .IsRequired();

            builder.Property(c => c.OldData)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.NewData)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();
            builder.Property(c => c.Remark)
               .HasMaxLength(256);

            builder.HasIndex(c => new { c.RoundId })
                    .HasDatabaseName("ix_round_operation_records_round_id");

            builder.HasOne<Round>()
                .WithMany()
                .HasForeignKey(b => b.RoundId)
                .HasConstraintName("fk_round_operation_records_round_id")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
