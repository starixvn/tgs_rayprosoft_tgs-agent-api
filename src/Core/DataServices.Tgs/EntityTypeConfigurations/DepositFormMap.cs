using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            builder.Property(c => c.BankAccount)
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.BrandPaymentId)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder
                .HasOne(c => c.Player)
                .WithMany(p => p.DepositForms)
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_deposit_forms_players_playerCode");

            builder
                .HasOne(c => c.Brand)
                .WithMany(b => b.DepositForms)
                .HasForeignKey(c => c.BrandId)
                .HasConstraintName("fk_deposit_froms_brands_id");

            builder
                .HasOne(c => c.BrandPayment)
                .WithMany(bp => bp.DepositForms)
                .HasForeignKey(c => c.BrandPaymentId)
                .HasConstraintName("fk_deposit_forms_brand_payments_id");
        }
    }
}
