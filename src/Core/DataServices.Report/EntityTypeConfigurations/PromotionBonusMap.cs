using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report.Models;

namespace TGS.Core.DataServices.Report.EntityTypeConfigurations
{
    internal class PromotionBonusMap : IEntityTypeConfiguration<PromotionBonus>
    {
        void IEntityTypeConfiguration<PromotionBonus>.Configure(EntityTypeBuilder<PromotionBonus> builder)
        {
            builder.ToTable("promotion_bonus");

            builder.HasKey(c => c.Id)
                .HasName("pk_promotion_bonus_id");

            builder.Property(c => c.PromotionId)
                .IsRequired();

            builder.Property(c => c.From)
                .IsRequired();

            builder.Property(c => c.To)
                .IsRequired();

            builder.Property(c => c.PromotionType)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt);

            builder.Property(c => c.TotalAmountLimit)
                .HasPrecision(18, 6);

            builder.Property(c => c.Percentage)
                .HasPrecision(9, 6);

            builder.Property(c => c.BonusAmountLimit)
                .HasPrecision(18, 6);

            builder.Property(e => e.CreatedOn)
                .IsRequired();

            builder.HasMany(x => x.Players)
                .WithOne(x => x.PromotionBonus)
                .HasForeignKey(x => x.PromotionBonusId)
                .HasConstraintName("fk_promotion_bonus_promotion_brand_id");

            builder.HasIndex(c => new {c.PromotionId})
                .HasDatabaseName("ix_promotion_bonus_promotion_id");

            builder.HasIndex(c => new {c.PromotionId, c.From, c.To})
                .HasDatabaseName("ix_promotion_bonus_multiple_keys");
        }
    }
}