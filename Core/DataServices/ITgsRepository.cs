using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
{
    public interface ITgsRepository : IBaseRepository
    {
        DbSet<Brand> Brands { get; set; }

        DbSet<Currency> Currencies { get; set; }

        DbSet<Language> Languages { get; set; }

        DbSet<Reseller> Resellers { get; set; }

        DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }

        DbSet<ResellerCurrency> ResellerCurrencies { get; set; }

        DbSet<ResellerLanguage> ResellerLanguages { get; set; }

        DbSet<Game> Games { get; set; }

        DbSet<GameProvider> GameProviders { get; set; }

        DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

        DbSet<BrandGame> BrandGames { get; set; }

        DbSet<LocalizedString> LocalizedStrings { get; set; }

        DbSet<Player> Players { get; set; }

        DbSet<Wallet> Wallets { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Whitelist> Whitelists { get; set; }

        DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }

        DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }

        DbSet<BrandCurrency> BrandCurrencies { get; set; }

        DbSet<BrandLanguage> BrandBrandLanguages { get; set; }

        DbSet<BrandGameProviderConfiguration> BrandGameProviderConfigurations { get; set; }

        DbSet<BetLimitGroup> BetLimitGroups { get; set; }

        DbSet<BrandBetLimit> BrandBetLimits { get; set; }

        DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }
    }
}
