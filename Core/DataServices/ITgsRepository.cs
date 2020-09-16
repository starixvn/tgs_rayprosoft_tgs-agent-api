using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
{
    public interface ITgsRepository : IBaseRepository
    {
        DbSet<Licensee> Licensees { get; set; }

        DbSet<Currency> Currencies { get; set; }

        DbSet<Language> Languages { get; set; }

        DbSet<Reseller> Resellers { get; set; }

        DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }

        DbSet<ResellerCurrency> ResellerCurrencies { get; set; }

        DbSet<ResellerLanguage> ResellerLanguages { get; set; }

        DbSet<Game> Games { get; set; }

        DbSet<GameProvider> GameProviders { get; set; }

        DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

        DbSet<LicenseeGame> LicenseeGames { get; set; }

        DbSet<LocalizedString> LocalizedStrings { get; set; }

        DbSet<Player> Players { get; set; }

        DbSet<Wallet> Wallets { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Whitelist> Whitelists { get; set; }

        DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }

        DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }

        DbSet<LicenseeCurrency> LicenseeCurrencies { get; set; }

        DbSet<LicenseeLanguage> LicenseeLicenseeLanguages { get; set; }

        DbSet<LicenseeGameProviderConfiguration> LicenseeGameProviderConfigurations { get; set; }

        DbSet<BetLimitGroup> BetLimitGroups { get; set; }

        DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }

        DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }

        DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        DbSet<PlayerBank> PlayerBanks { get; set; }
        DbSet<VipLevel> VipLevels { get; set; }
    }
}
