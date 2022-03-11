using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerIncreaseAmountMap : IEntityTypeConfiguration<PlayerIncreaseAmount>
    {
        void IEntityTypeConfiguration<PlayerIncreaseAmount>.Configure(EntityTypeBuilder<PlayerIncreaseAmount> builder)
        {
            builder.ToTable("player_increase_amounts");

            builder.HasKey(bc => bc.Id)
                .HasName("pk_player_increase_amounts_id");

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.PlayerIncreaseAmountType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6);

            builder.Property(c => c.BucketAmount)
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
                    c.PlayerIncreaseAmountType
                })
                .HasDatabaseName("ix_player_increase_amount_multiple_keys");
        }
    }
}