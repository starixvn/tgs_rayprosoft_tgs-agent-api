using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.EntityTypeConfigurations;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.DataServices
{
    public class TgsRepository : DbContext, ITgsRepository
    {
        DbSet<Reseller> ITgsRepository.Resellers { get; set; }
        DbSet<Game> ITgsRepository.Games { get; set; }
        DbSet<GameProvider> ITgsRepository.GameProviders { get; set; }
        DbSet<GameProviderConfiguration> ITgsRepository.GameProviderConfigurations { get; set; }
        DbSet<ResellerGameProvider> ITgsRepository.ResellerGameProviders { get; set; }
        DbSet<LicenseeCurrency> ITgsRepository.LicenseeCurrencies { get; set; }
        DbSet<LicenseeLanguage> ITgsRepository.LicenseeLanguages { get; set; }
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

            modelBuilder.ApplyConfiguration(new CurrencyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameProviderTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GpcTypeConfiguration());

            modelBuilder.ApplyConfiguration(new ResellerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResellerGameProviderTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseeCurrencyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseeLanguageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResellerCurrencyTypeConfiguration());

            modelBuilder.ApplyConfiguration(new ResellerLanguageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BrandTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BrandGameTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerTypeConfiguration());

            modelBuilder.ApplyConfiguration(new WhitelistTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WalletTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SettingTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LocalizedStringTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BrandCurrencyTypeConfiguration());

            modelBuilder.ApplyConfiguration(new GameProviderCurrencyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameProviderLanguageTypeConfiguration());
        }
    }
}
