using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.EntityTypeConfigurations;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
{
    public class TgsRepository : DbContext, ITgsRepository
    {
        DbSet<Reseller> ITgsRepository.Resellers { get; set; }
        DbSet<Game> ITgsRepository.Games { get; set; }
        DbSet<GameProvider> ITgsRepository.GameProviders { get; set; }
        DbSet<GameProviderConfiguration> ITgsRepository.GameProviderConfigurations { get; set; }
        DbSet<ResellerGameProvider> ITgsRepository.ResellerGameProviders { get; set; }
        DbSet<ResellerCurrency> ITgsRepository.ResellerCurrencies { get; set; }
        DbSet<ResellerLanguage> ITgsRepository.ResellerLanguages { get; set; }
        DbSet<Currency> ITgsRepository.Currencies { get; set; }
        DbSet<Language> ITgsRepository.Languages { get; set; }
        DbSet<LicenseeGame> ITgsRepository.LicenseeGames { get; set; }
        DbSet<LocalizedString> ITgsRepository.LocalizedStrings { get; set; }
        DbSet<Player> ITgsRepository.Players { get; set; }
        DbSet<Wallet> ITgsRepository.Wallets { get; set; }
        DbSet<Setting> ITgsRepository.Settings { get; set; }
        DbSet<Whitelist> ITgsRepository.Whitelists { get; set; }
        DbSet<GameProviderCurrency> ITgsRepository.GameProviderCurrencies { get; set; }
        DbSet<GameProviderLanguage> ITgsRepository.GameProviderLanguages { get; set; }
        DbSet<LicenseeCurrency> ITgsRepository.LicenseeCurrencies { get; set; }
        DbSet<LicenseeLanguage> ITgsRepository.LicenseeLicenseeLanguages { get; set; }
        DbSet<Licensee> ITgsRepository.Licensees { get; set; }
        DbSet<LicenseeGameProviderConfiguration> ITgsRepository.LicenseeGameProviderConfigurations { get; set; }
        DbSet<BetLimitGroup> ITgsRepository.BetLimitGroups { get; set; }
        DbSet<LicenseeBetLimit> ITgsRepository.LicenseeBetLimits { get; set; }
        DbSet<GameProviderBetLimit> ITgsRepository.GameProviderBetLimits { get; set; }
        DbSet<PlayerLoginRecord> ITgsRepository.PlayerLoginRecords { get; set; }

        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {

        }

        Task<bool> IBaseRepository.CanConnectionAsync()
        {
            return Database.CanConnectAsync();
        }

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level)
        {
            return Database.BeginTransactionAsync(level);
        }

        IDbConnection IBaseRepository.GetDbConnection()
        {
            return Database.GetDbConnection();
        }
        Task IBaseRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new CurrencyMap());
            modelBuilder.ApplyConfiguration(new LanguageMap());
            modelBuilder.ApplyConfiguration(new GameMap());
            modelBuilder.ApplyConfiguration(new GameProviderMap());
            modelBuilder.ApplyConfiguration(new GameProviderConfigurationMap());

            modelBuilder.ApplyConfiguration(new ResellerMap());
            modelBuilder.ApplyConfiguration(new ResellerGameProviderMap());
            modelBuilder.ApplyConfiguration(new ResellerCurrencyMap());

            modelBuilder.ApplyConfiguration(new ResellerLanguageMap());
            modelBuilder.ApplyConfiguration(new LicenseeMap());
            modelBuilder.ApplyConfiguration(new LicenseeGameMap());
            modelBuilder.ApplyConfiguration(new LicenseeGameProviderConfigurationMap());
            modelBuilder.ApplyConfiguration(new PlayerMap());

            modelBuilder.ApplyConfiguration(new WhitelistTypeMap());
            modelBuilder.ApplyConfiguration(new WalletTypeMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
            modelBuilder.ApplyConfiguration(new LocalizedStringMap());
            modelBuilder.ApplyConfiguration(new LicenseeCurrencyMap());
            modelBuilder.ApplyConfiguration(new LicenseeLanguageMap());

            modelBuilder.ApplyConfiguration(new GameProviderCurrencyMap());
            modelBuilder.ApplyConfiguration(new GameProviderLanguageMap());

            modelBuilder.ApplyConfiguration(new BetLimitGroupMap());
            modelBuilder.ApplyConfiguration(new LicenseeBetLimitMap());
            modelBuilder.ApplyConfiguration(new GameProviderBetLimitMap());

            modelBuilder.ApplyConfiguration(new PlayerLoginRecordMap());
        }
    }
}
