using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class TransactionPromotionMap : IEntityTypeConfiguration<TransactionPromotion>
    {
        void IEntityTypeConfiguration<TransactionPromotion>.Configure(EntityTypeBuilder<TransactionPromotion> builder)
        {
            builder.ToTable("transaction_promotions");

            builder.HasKey(e => new {e.TransactionId, e.PromotionId})
                .HasName("pk_transaction_promotions");

            builder
                .HasOne(e => e.Transaction)
                .WithMany()
                .HasForeignKey(e => e.TransactionId)
                .HasConstraintName("fk_transaction_promotion_transaction_id");

            builder.HasIndex(e => e.TransactionId)
                .HasDatabaseName("ix_transactions_transaction_id");

            builder
                .HasOne(e => e.Promotion)
                .WithMany()
                .HasForeignKey(e => e.PromotionId)
                .HasConstraintName("fk_transaction_promotion_promotion_id");

            builder.HasIndex(e => e.PromotionId)
                .HasDatabaseName("ix_transaction_Promotion_id");
        }
    }
}