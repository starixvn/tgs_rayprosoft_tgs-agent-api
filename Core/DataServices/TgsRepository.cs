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
        DbSet<BrandGame> ITgsRepository.BrandGames { get; set; }
        DbSet<LocalizedString> ITgsRepository.LocalizedStrings { get; set; }
        DbSet<Player> ITgsRepository.Players { get; set; }
        DbSet<Wallet> ITgsRepository.Wallets { get; set; }
        DbSet<Setting> ITgsRepository.Settings { get; set; }
        DbSet<Whitelist> ITgsRepository.Whitelists { get; set; }
        DbSet<GameProviderCurrency> ITgsRepository.GameProviderCurrencies { get; set; }
        DbSet<GameProviderLanguage> ITgsRepository.GameProviderLanguages { get; set; }
        DbSet<BrandCurrency> ITgsRepository.BrandCurrencies { get; set; }
        DbSet<BrandLanguage> ITgsRepository.BrandBrandLanguages { get; set; }
        DbSet<Brand> ITgsRepository.Brands { get; set; }
        DbSet<BrandGameProviderConfiguration> ITgsRepository.BrandGameProviderConfigurations { get; set; }
        DbSet<BetLimitGroup> ITgsRepository.BetLimitGroups { get; set; }
        DbSet<BrandBetLimit> ITgsRepository.BrandBetLimits { get; set; }
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
            modelBuilder.ApplyConfiguration(new BrandMap());
            modelBuilder.ApplyConfiguration(new BrandGameMap());
            modelBuilder.ApplyConfiguration(new BrandGameProviderConfigurationMap());
            modelBuilder.ApplyConfiguration(new PlayerMap());

            modelBuilder.ApplyConfiguration(new WhitelistTypeMap());
            modelBuilder.ApplyConfiguration(new WalletTypeMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
            modelBuilder.ApplyConfiguration(new LocalizedStringMap());
            modelBuilder.ApplyConfiguration(new BrandCurrencyMap());
            modelBuilder.ApplyConfiguration(new BrandLanguageMap());

            modelBuilder.ApplyConfiguration(new GameProviderCurrencyMap());
            modelBuilder.ApplyConfiguration(new GameProviderLanguageMap());

            modelBuilder.ApplyConfiguration(new BetLimitGroupMap());
            modelBuilder.ApplyConfiguration(new BrandBetLimitMap());
            modelBuilder.ApplyConfiguration(new GameProviderBetLimitMap());

            modelBuilder.ApplyConfiguration(new PlayerLoginRecordMap());
        }
    }
}
