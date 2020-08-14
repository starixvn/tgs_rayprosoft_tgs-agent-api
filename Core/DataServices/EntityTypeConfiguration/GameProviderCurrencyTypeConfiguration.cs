using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class GameProviderCurrencyTypeConfiguration : IEntityTypeConfiguration<GameProviderCurrency>
    {
        void IEntityTypeConfiguration<GameProviderCurrency>.Configure(EntityTypeBuilder<GameProviderCurrency> builder)
        {
            builder.ToTable("GameProviderCurrencies", "dbo");
            builder.HasKey(bc => new { bc.GameProviderId, bc.CurrencyCode });
            builder
                .HasOne(bc => bc.GameProvider)
                .WithMany(c => c.Currencies)
                .HasForeignKey(bc => bc.GameProviderId);
            builder
                .HasOne(bc => bc.Currency)
                .WithMany(c => c.GameProviderCurrencies)
                .HasForeignKey(bc => bc.CurrencyCode);
        }
    }
}
