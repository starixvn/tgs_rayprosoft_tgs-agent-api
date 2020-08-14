using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class BrandCurrencyTypeConfiguration : IEntityTypeConfiguration<BrandCurrency>
    {
        void IEntityTypeConfiguration<BrandCurrency>.Configure(EntityTypeBuilder<BrandCurrency> builder)
        {
            builder.ToTable("BrandCurrencies", "dbo");
            builder.HasKey(bc => bc.Id);
            builder
                .HasOne(bc => bc.Brand)
                .WithMany(c => c.BrandCurrencies)
                .HasForeignKey(bc => bc.BrandId);

            builder
                .HasOne(bc => bc.Currency)
                .WithMany(c => c.BrandCurrencies)
                .HasForeignKey(bc => bc.CurrencyCode);

            builder.Property(c => c.BrandCurrencyCode)
                .HasColumnType("nvarchar(16)");

        }
    }
}
