using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandCurrencyMap : IEntityTypeConfiguration<BrandCurrency>
    {
        void IEntityTypeConfiguration<BrandCurrency>.Configure(EntityTypeBuilder<BrandCurrency> builder)
        {
            builder.ToTable("brand_currencies");

            builder.HasKey(e => new {e.BrandId, e.CurrencyCode});

            builder.Property(e => e.CurrencyCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(e => e.BrandCurrencyCode)
                .HasMaxLength(32)
                .IsRequired();

            builder.HasOne(e => e.Brand)
                .WithMany(e => e.Currencies)
                .HasForeignKey(e => e.BrandId)
                .HasConstraintName("fk_brand_currencies_brands_brand_id");

            builder.HasOne(e => e.Currency)
                .WithMany(e => e.BrandCurrencies)
                .HasForeignKey(e => e.CurrencyCode)
                .HasConstraintName("fk_brand_currencies_currencies_currency_code");

            builder.HasIndex(e => e.CurrencyCode)
                .HasDatabaseName("ix_brand_currencies_currency_code");
        }
    }
}