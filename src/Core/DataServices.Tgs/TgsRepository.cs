using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.Bulk;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.DataInitializers;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;

namespace TGS.Core.DataServices.Tgs
{
    public interface ITgsRepository : IBaseRepository
    {
        DbSet<Reseller> Resellers { get; set; }
        DbSet<Licensee> Licensees { get; set; }
        DbSet<Brand> Brands { get; set; }
        DbSet<Currency> Currencies { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Game> Games { get; set; }
        DbSet<GameProvider> GameProviders { get; set; }
        DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }
        DbSet<BrandGroup> BrandGroups { get; set; }
        DbSet<BrandGame> BrandGames { get; set; }
        DbSet<BrandGroupGame> BrandGroupGames { get; set; }
        DbSet<LocalizedString> LocalizedStrings { get; set; }
        DbSet<Player> Players { get; set; }
        DbSet<Wallet> Wallets { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<Whitelist> Whitelists { get; set; }
        DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }
        DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }
        DbSet<ResellerCurrency> ResellerCurrencies { get; set; }
        DbSet<ResellerLanguage> ResellerLanguages { get; set; }
        DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }
        DbSet<LicenseeCurrency> LicenseeCurrencies { get; set; }
        DbSet<LicenseeLanguage> LicenseeLanguages { get; set; }
        DbSet<LicenseeGameProvider> LicenseeGameProviders { get; set; }
        DbSet<BrandCurrency> BrandCurrencies { get; set; }
        DbSet<BrandLanguage> BrandLanguages { get; set; }
        DbSet<BrandGameProvider> BrandGameProviders { get; set; }
        DbSet<BetLimit> BetLimits { get; set; }
        DbSet<ResellerBetLimit> ResellerBetLimits { get; set; }
        DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }
        DbSet<BrandBetLimit> BrandBetLimits { get; set; }
        DbSet<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }
        DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }
        DbSet<BrandGroupGameProviderBetLimit> BrandGroupGameProviderBetLimits { get; set; }
        DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        DbSet<PlayerBank> PlayerBanks { get; set; }
        DbSet<VipLevel> VipLevels { get; set; }
        DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }
        DbSet<Announcement> Announcements { get; set; }
        DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }
        DbSet<StationLetter> StationLetters { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<PaymentMethod> PaymentMethods { get; set; }
        DbSet<PaymentChannel> PaymentChannels { get; set; }
        DbSet<BrandPayment> BrandPayments { get; set; }
        DbSet<BrandPaymentPlayer> PaymentPlayerList { get; set; }
        DbSet<BrandPaymentVipLevel> PaymentVipLevels { get; set; }
        DbSet<BrandPaymentAuditLog> PaymentAuditLogs { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<SportType> SportTypes { get; set; }
        DbSet<MarketType> MarketTypes { get; set; }
        DbSet<SportMarketType> SportMarketTypes { get; set; }
        DbSet<PlayerSportsBetLimit> PlayerSportsBetLimits { get; set; }
        DbSet<Round> Rounds { get; set; }
        DbSet<RoundDetail> RoundDetails { get; set; }
        DbSet<GameAction> GameActions { get; set; }
        DbSet<GameActionBatch> GameActionBatches { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }

    public class TgsRepository : DbContext, ITgsRepository
    {
        private readonly NpgsqlBulkUploader _uploader;

        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {
            _uploader = new NpgsqlBulkUploader(this);
        }

        public DbSet<Reseller> Resellers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameProvider> GameProviders { get; set; }
        public DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }
        public DbSet<BrandGameProvider> BrandGameProviders { get; set; }
        public DbSet<ResellerCurrency> ResellerCurrencies { get; set; }
        public DbSet<ResellerLanguage> ResellerLanguages { get; set; }
        public DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<BrandGame> BrandGames { get; set; }
        public DbSet<BrandGroup> BrandGroups { get; set; }
        public DbSet<BrandGroupGame> BrandGroupGames { get; set; }
        public DbSet<LocalizedString> LocalizedStrings { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Whitelist> Whitelists { get; set; }
        public DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }
        public DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }
        public DbSet<LicenseeCurrency> LicenseeCurrencies { get; set; }
        public DbSet<LicenseeLanguage> LicenseeLanguages { get; set; }
        public DbSet<LicenseeGameProvider> LicenseeGameProviders { get; set; }
        public DbSet<Licensee> Licensees { get; set; }
        public DbSet<BetLimit> BetLimits { get; set; }
        public DbSet<ResellerBetLimit> ResellerBetLimits { get; set; }
        public DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }
        public DbSet<BrandBetLimit> BrandBetLimits { get; set; }
        public DbSet<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }
        public DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }
        public DbSet<BrandGroupGameProviderBetLimit> BrandGroupGameProviderBetLimits { get; set; }
        public DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        public DbSet<PlayerBank> PlayerBanks { get; set; }
        public DbSet<VipLevel> VipLevels { get; set; }
        public DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }
        public DbSet<StationLetter> StationLetters { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentChannel> PaymentChannels { get; set; }
        public DbSet<BrandPayment> BrandPayments { get; set; }
        public DbSet<BrandPaymentPlayer> PaymentPlayerList { get; set; }
        public DbSet<BrandPaymentVipLevel> PaymentVipLevels { get; set; }
        public DbSet<BrandPaymentAuditLog> PaymentAuditLogs { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandCurrency> BrandCurrencies { get; set; }
        public DbSet<BrandLanguage> BrandLanguages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SportType> SportTypes { get; set; }
        public DbSet<MarketType> MarketTypes { get; set; }
        public DbSet<SportMarketType> SportMarketTypes { get; set; }
        public DbSet<PlayerSportsBetLimit> PlayerSportsBetLimits { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<RoundDetail> RoundDetails { get; set; }
        public DbSet<GameAction> GameActions { get; set; }
        public DbSet<GameActionBatch> GameActionBatches { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public IDbContextTransaction CurrentTransaction =>
            Database.CurrentTransaction;

        public Task<bool> CanConnectionAsync() => Database.CanConnectAsync();

        public void SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        public IDbConnection GetDbConnection() =>
            Database.GetDbConnection();

        Task IBaseRepository.BulkInsertAsync<T>(IEnumerable<T> entities) =>
            _uploader.InsertAsync(entities);

        Task IBaseRepository.BulkUpdateAsync<T>(IEnumerable<T> entities) =>
            _uploader.UpdateAsync(entities);

        public async Task ExecuteMigrationAndSeedingAsync(IWebHostEnvironment env)
        {
            var isMigrate = !await Database.GetService<IRelationalDatabaseCreator>().ExistsAsync() ||
                            (await Database.GetPendingMigrationsAsync()).Any();

            if (isMigrate)
            {
                await Database.MigrateAsync();
            }

            if (env.IsDevelopmentOrDocker())
            {
                IDataInitializer data = new DataInitializer(this, env);
                await data.SetInitialDataAsync();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Consts.Db.Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.HasPostgresExtension("pg_trgm");
        }
    }
}