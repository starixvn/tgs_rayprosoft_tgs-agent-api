using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerReduceAmountMap : IEntityTypeConfiguration<PlayerReduceAmount>
    {
        void IEntityTypeConfiguration<PlayerReduceAmount>.Configure(EntityTypeBuilder<PlayerReduceAmount> builder)
        {
            builder.ToTable("player_reduce_amounts");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_player_reduce_amounts_id");

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(p => p.TransferTarget);

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6);

            builder.Property(p => p.Remark);

            builder.Property(l => l.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(l => l.CreatedOn)
                .IsRequired();

            builder.Property(l => l.UpdatedOn)
                .IsRequired();

            builder.HasIndex(c => new
                {
                    c.CreatedOn,
                    c.BrandId,
                    c.PlayerCode,
                    c.Type
                })
                .HasDatabaseName("ix_player_reduce_amount_multiple_keys");
        }
    }
}