using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PaymentHandlingFeeMap : IEntityTypeConfiguration<PaymentHandlingFee>
    {
        public void Configure(EntityTypeBuilder<PaymentHandlingFee> builder)
        {
            builder.ToTable("payment_handling_fees");
            builder.HasKey(phf => phf.Id)
                .HasName("pk_payment_handling_fees_id");

            builder.Property(phf => phf.Id)
                .IsRequired();
            builder.Property(phf => phf.Type)
                .IsRequired();
            builder.Property(phf => phf.Amount)
                .IsRequired();
            builder.Property(phf => phf.Fee)
                .IsRequired();
            builder.Property(phf => phf.FormId)
                .IsRequired();
            builder.Property(phf => phf.UserType)
                .IsRequired();
            builder.Property(phf => phf.UserId)
                .IsRequired();
            builder.Property(phf => phf.CreatedOn)
                .IsRequired()
                .HasColumnType("timestamptz");
            builder.Property(phf => phf.ApprovedOn)
                .IsRequired()
                .HasColumnType("timestamptz");
            builder.Property(phf => phf.RecordedOn)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("Now()");
            builder.Property(phf => phf.Remark)
                .HasColumnType(ColumnTypes.Jsonb);
            builder.Property(phf => phf.BrandId)
                .IsRequired();

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(phf => phf.BrandId)
                .HasConstraintName("fk_payment_handling_fees_brands_brand_id")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(phf => phf.FormId)
                .HasDatabaseName("ix_payment_handling_fees_form_id")
                .IncludeProperties(phf => new { phf.Amount, phf.Fee, phf.RecordedOn });

            builder.HasIndex(phf => new { phf.RecordedOn, phf.FormId })
                .HasDatabaseName("ix_payment_handling_fees_recorded_on_form_id")
                .IncludeProperties(phf => new { phf.Amount, phf.Fee });
        }
    }
}
