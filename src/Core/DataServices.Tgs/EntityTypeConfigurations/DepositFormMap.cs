using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class DepositFormMap : IEntityTypeConfiguration<DepositForm>
    {
        public void Configure(EntityTypeBuilder<DepositForm> builder)
        {
            builder.ToTable("deposit_forms");

            builder.HasKey(c => c.Id)
                .HasName("pk_deposit_forms");

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.BrandPaymentId)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.Remark);

            builder.Property(c => c.ApplyInfo)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(c => c.PaymentReturn)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(c => c.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.Property(c => c.UpdatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder
                .HasOne(c => c.Player)
                .WithMany()
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_deposit_forms_players_playerCode");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_deposit_forms_playerCode");

            builder
                .HasOne(c => c.Brand)
                .WithMany()
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_deposit_froms_brands_id");

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_deposit_forms_brandId");

            builder
                .HasOne(c => c.BrandPayment)
                .WithMany()
                .HasForeignKey(c => c.BrandPaymentId)
                .HasConstraintName("fk_deposit_forms_brand_payments_id");

            builder.HasIndex(c => c.BrandPaymentId)
                .HasDatabaseName("ix_deposit_forms_brandPaymentId");
        }
    }
}
