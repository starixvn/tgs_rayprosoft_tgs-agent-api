using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class WithdrawFormMap : IEntityTypeConfiguration<WithdrawForm>
    {
        public void Configure(EntityTypeBuilder<WithdrawForm> builder)
        {
            builder.ToTable("withdraw_forms");

            builder.HasKey(w => w.Id)
                .HasName("pk_withdraw_forms");

            builder.Property(w => w.BrandId)
                .IsRequired();

            builder.Property(w => w.PlayerCode)
                .IsRequired();

            builder.Property(w => w.Amount)
                .IsRequired();

            builder.Property(w => w.BrandPaymentId);

            builder.Property(w => w.Status)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(w => w.Remark);

            builder.Property(w => w.BankInfoId)
                .IsRequired();

            builder.Property(w => w.BankInfo)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(w => w.ApplyInfo)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(w => w.PaymentReturn)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(w => w.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.Property(w => w.UpdatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.HasOne(w => w.Player)
                .WithMany()
                .HasForeignKey(w => w.PlayerCode)
                .HasConstraintName("fk_withdraw_forms_players_player_code");

            builder.HasIndex(w => w.PlayerCode)
                .HasDatabaseName("ix_withdraw_forms_player_code");

            builder.HasOne(w => w.Brand)
                .WithMany()
                .HasForeignKey(w => w.BrandId)
                .HasConstraintName("fk_withdraw_forms_brands_brand_id");

            builder.HasIndex(w => w.BrandId)
                .HasDatabaseName("ix_withdraw_forms_brand_id");

            builder.HasOne(w => w.BrandPayment)
                .WithMany()
                .HasForeignKey(w => w.BrandPaymentId)
                .HasConstraintName("fk_withdraw_forms_brand_payments_brand_payment_id");

            builder.HasIndex(w => w.BrandPaymentId)
                .HasDatabaseName("ix_withdraw_froms_brand_payment_id");
        }
    }
}
