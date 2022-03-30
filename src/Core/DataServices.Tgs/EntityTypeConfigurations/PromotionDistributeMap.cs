using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PromotionDistributeMap : IEntityTypeConfiguration<PromotionDistribute>
    {
        public void Configure(EntityTypeBuilder<PromotionDistribute> builder)
        {
            builder.ToTable("promotion_distributes");

            builder.HasKey(pd => pd.Id)
                .HasName("pk_promotion_distributes");

            builder.Property(pd => pd.PlanId);
            builder.Property(pd => pd.PlayerCode);
            builder.Property(pd => pd.BrandId);
            builder.Property(pd => pd.PromotionId);
            builder.Property(pd => pd.PromotionType);
            builder.Property(pd => pd.BonusAmount);
            builder.Property(pd => pd.Distributor);
            builder.Property(pd => pd.SentOn);
            builder.Property(pd => pd.CreatedOn);

            builder.HasOne<Promotion>()
                .WithMany()
                .HasForeignKey(pd => pd.PromotionId)
                .HasConstraintName("fk_promotion_distributes_promotions")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pd => new { pd.PlayerCode, pd.PlanId })
                .HasFilter("plan_id IS NOT NULL")
                .IsUnique()
                .HasDatabaseName("ux_promotion_distributes");
        }
    }
}
