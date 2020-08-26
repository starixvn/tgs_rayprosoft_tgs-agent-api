using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandCurrencyMap : IEntityTypeConfiguration<BrandCurrency>
    {
        void IEntityTypeConfiguration<BrandCurrency>.Configure(EntityTypeBuilder<BrandCurrency> builder)
        {
            builder.ToTable("BrandCurrencies", "dbo");
            builder.HasKey(bc => new {bc.BrandId, bc.CurrencyCode});
            builder
                .HasOne(bc => bc.Brand)
                .WithMany(c => c.BrandCurrencies)
                .HasForeignKey(bc => bc.BrandId);

            builder
                .HasOne(bc => bc.Currency)
                .WithMany(c => c.BrandCurrencies)
                .HasForeignKey(bc => bc.CurrencyCode);

            builder.Property(c => c.CurrencyCode)
                .HasColumnType("nvarchar(16)")
                .IsUnicode()
                .IsRequired();

            builder.Property(c => c.BrandCurrencyCode)
                .HasColumnType("nvarchar(16)")
                .IsUnicode()
                .IsRequired();
        }
    }
}
