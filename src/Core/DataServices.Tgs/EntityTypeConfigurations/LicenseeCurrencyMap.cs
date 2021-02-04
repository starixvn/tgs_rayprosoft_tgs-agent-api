using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeCurrencyMap : IEntityTypeConfiguration<LicenseeCurrency>
    {
        void IEntityTypeConfiguration<LicenseeCurrency>.Configure(EntityTypeBuilder<LicenseeCurrency> builder)
        {
            builder.ToTable("licensee_currencies");

            builder.HasKey(e => new {e.LicenseeId, e.CurrencyCode})
                .HasName("pk_licensee_currencies");

            builder.Property(e => e.CurrencyCode)
                .HasMaxLength(16)
                .IsRequired();

            builder
                .HasOne(e => e.Licensee)
                .WithMany(e => e.Currencies)
                .HasForeignKey(e => e.LicenseeId)
                .HasConstraintName("fk_licensee_currencies_licensees_licensee_id");

            builder
                .HasOne(e => e.Currency)
                .WithMany(e => e.LicenseeCurrencies)
                .HasForeignKey(e => e.CurrencyCode)
                .HasConstraintName("fk_licensee_currencies_currencies_currency_code");

            builder.HasIndex(e => e.CurrencyCode)
                .HasDatabaseName("ix_licensee_currencies_currency_code");
        }
    }
}