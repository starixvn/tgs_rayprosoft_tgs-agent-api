using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionMap : IEntityTypeConfiguration<Promotion>
    {
        void IEntityTypeConfiguration<Promotion>.Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("promotions");

            builder.HasKey(b => b.Id)
                .HasName("pk_promotions_id");

            builder.Property(c => c.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.From)
                .IsRequired();

            builder.Property(c => c.To)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.PromotionType)
                .IsRequired();

            builder.Property(c => c.Unlimited)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired(false);

            builder.Property(c => c.BonusAmountLimit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.AmountUsed)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.Percentage)
                .HasPrecision(9, 6)
                .IsRequired();

            builder.Property(c => c.TotalAmountLimit)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.TurnoverMultiple)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.Subject)
                .HasMaxLength(512);

            builder.Property(c => c.SubSubject)
                .HasMaxLength(2048);

            builder.Property(c => c.Localization)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .HasConstraintName("fk_promotions_brands_brand_id");

            builder.HasMany(p => p.PromotionGameProviders)
                .WithOne(p => p.Promotion)
                .HasForeignKey(p => p.PromotionId);

            builder.HasMany(p => p.PromotionVipLevels)
                .WithOne(p => p.Promotion)
                .HasForeignKey(p => p.PromotionId);

            builder.HasMany(p => p.PromotionParticipants)
                .WithOne(p => p.Promotion)
                .HasForeignKey(p => p.PromotionId);

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_promotions_brand_id");
        }
    }
}