using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class GameProviderCurrencyTypeConfiguration : IEntityTypeConfiguration<GameProviderCurrency>
    {
        void IEntityTypeConfiguration<GameProviderCurrency>.Configure(EntityTypeBuilder<GameProviderCurrency> builder)
        {
            builder.ToTable("GameProviderCurrencies", "dbo");
            builder.HasKey(gpc => new { gpc.GameProviderId, gpc.CurrencyCode });
            
            builder
                .HasOne(gpc => gpc.GameProvider)
                .WithMany(gp => gp.Currencies)
                .HasForeignKey(gpc => gpc.GameProviderId);

            builder
                .HasOne(gpc => gpc.Currency)
                .WithMany(c => c.GameProviderCurrencies)
                .HasForeignKey(gpc => gpc.CurrencyCode);

            builder.Property(gpc => gpc.CurrencyCode)
                .HasColumnType("nvarchar(16)")
                .IsUnicode();

            builder.Property(gpc => gpc.GameProviderCurrencyCode)
                .HasColumnType("nvarchar(16)")
                .IsUnicode()
                .IsRequired();
        }
    }
}
