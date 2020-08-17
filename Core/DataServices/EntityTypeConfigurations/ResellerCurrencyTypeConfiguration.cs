using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class ResellerCurrencyTypeConfiguration : IEntityTypeConfiguration<ResellerCurrency>
    {
        void IEntityTypeConfiguration<ResellerCurrency>.Configure(EntityTypeBuilder<ResellerCurrency> builder)
        {
            builder.ToTable("ResellerCurrencies", "dbo");
            builder.HasKey(bc => new { bc.ResellerId, bc.CurrencyCode });
            builder
                .HasOne(bc => bc.Reseller)
                .WithMany(c => c.Currencies)
                .HasForeignKey(bc => bc.ResellerId);
            builder
                .HasOne(bc => bc.Currency)
                .WithMany(c => c.ResellerCurrencies)
                .HasForeignKey(bc => bc.CurrencyCode);
        }
    }
}
