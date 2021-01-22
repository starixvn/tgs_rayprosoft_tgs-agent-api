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
    }

    public class TgsRepository : DbContext, ITgsRepository
    {
        private readonly NpgsqlBulkUploader _uploader;

        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {
            _uploader = new NpgsqlBulkUploader(this);
        }

        DbSet<Reseller> ITgsRepository.Resellers { get; set; }
        DbSet<Game> ITgsRepository.Games { get; set; }
        DbSet<GameProvider> ITgsRepository.GameProviders { get; set; }
        DbSet<GameProviderConfiguration> ITgsRepository.GameProviderConfigurations { get; set; }
        DbSet<BrandGameProvider> ITgsRepository.BrandGameProviders { get; set; }
        DbSet<ResellerCurrency> ITgsRepository.ResellerCurrencies { get; set; }
        DbSet<ResellerLanguage> ITgsRepository.ResellerLanguages { get; set; }
        DbSet<ResellerGameProvider> ITgsRepository.ResellerGameProviders { get; set; }
        DbSet<Currency> ITgsRepository.Currencies { get; set; }
        DbSet<Language> ITgsRepository.Languages { get; set; }
        DbSet<BrandGame> ITgsRepository.BrandGames { get; set; }
        DbSet<BrandGroup> ITgsRepository.BrandGroups { get; set; }
        DbSet<BrandGroupGame> ITgsRepository.BrandGroupGames { get; set; }
        DbSet<LocalizedString> ITgsRepository.LocalizedStrings { get; set; }
        DbSet<Player> ITgsRepository.Players { get; set; }
        DbSet<Wallet> ITgsRepository.Wallets { get; set; }
        DbSet<Setting> ITgsRepository.Settings { get; set; }
        DbSet<Whitelist> ITgsRepository.Whitelists { get; set; }
        DbSet<GameProviderCurrency> ITgsRepository.GameProviderCurrencies { get; set; }
        DbSet<GameProviderLanguage> ITgsRepository.GameProviderLanguages { get; set; }
        DbSet<LicenseeCurrency> ITgsRepository.LicenseeCurrencies { get; set; }
        DbSet<LicenseeLanguage> ITgsRepository.LicenseeLanguages { get; set; }
        DbSet<LicenseeGameProvider> ITgsRepository.LicenseeGameProviders { get; set; }
        DbSet<Licensee> ITgsRepository.Licensees { get; set; }
        DbSet<BetLimit> ITgsRepository.BetLimits { get; set; }
        DbSet<ResellerBetLimit> ITgsRepository.ResellerBetLimits { get; set; }
        DbSet<LicenseeBetLimit> ITgsRepository.LicenseeBetLimits { get; set; }
        DbSet<BrandBetLimit> ITgsRepository.BrandBetLimits { get; set; }
        DbSet<BrandGroupBetLimit> ITgsRepository.BrandGroupBetLimits { get; set; }
        DbSet<GameProviderBetLimit> ITgsRepository.GameProviderBetLimits { get; set; }
        DbSet<BrandGroupGameProviderBetLimit> ITgsRepository.BrandGroupGameProviderBetLimits { get; set; }
        DbSet<PlayerLoginRecord> ITgsRepository.PlayerLoginRecords { get; set; }
        DbSet<PlayerBank> ITgsRepository.PlayerBanks { get; set; }
        DbSet<VipLevel> ITgsRepository.VipLevels { get; set; }
        DbSet<PlayerAuditLog> ITgsRepository.PlayerAuditLogs { get; set; }
        DbSet<Announcement> ITgsRepository.Announcements { get; set; }
        DbSet<StationLetterPlayer> ITgsRepository.StationLetterPlayers { get; set; }
        DbSet<StationLetter> ITgsRepository.StationLetters { get; set; }
        DbSet<Payment> ITgsRepository.Payments { get; set; }
        DbSet<PaymentMethod> ITgsRepository.PaymentMethods { get; set; }
        DbSet<PaymentChannel> ITgsRepository.PaymentChannels { get; set; }
        DbSet<BrandPayment> ITgsRepository.BrandPayments { get; set; }
        DbSet<BrandPaymentPlayer> ITgsRepository.PaymentPlayerList { get; set; }
        DbSet<BrandPaymentVipLevel> ITgsRepository.PaymentVipLevels { get; set; }
        DbSet<BrandPaymentAuditLog> ITgsRepository.PaymentAuditLogs { get; set; }
        DbSet<Brand> ITgsRepository.Brands { get; set; }
        DbSet<BrandCurrency> ITgsRepository.BrandCurrencies { get; set; }
        DbSet<BrandLanguage> ITgsRepository.BrandLanguages { get; set; }
        DbSet<User> ITgsRepository.Users { get; set; }
        DbSet<Role> ITgsRepository.Roles { get; set; }
        DbSet<Department> ITgsRepository.Departments { get; set; }
        DbSet<SportType> ITgsRepository.SportTypes { get; set; }
        DbSet<MarketType> ITgsRepository.MarketTypes { get; set; }
        DbSet<SportMarketType> ITgsRepository.SportMarketTypes { get; set; }
        DbSet<PlayerSportsBetLimit> ITgsRepository.PlayerSportsBetLimits { get; set; }
        DbSet<Round> ITgsRepository.Rounds { get; set; }
        DbSet<RoundDetail> ITgsRepository.RoundDetails { get; set; }
        DbSet<GameAction> ITgsRepository.GameActions { get; set; }
        DbSet<GameActionBatch> ITgsRepository.GameActionBatches { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

        Task<bool> IBaseRepository.CanConnectionAsync() => Database.CanConnectAsync();

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        IDbConnection IBaseRepository.GetDbConnection() =>
            Database.GetDbConnection();

        Task IBaseRepository.SaveChangesAsync() => SaveChangesAsync();

        Task IBaseRepository.BulkInsertAsync<T>(IEnumerable<T> entities) =>
            _uploader.InsertAsync(entities);

        Task IBaseRepository.BulkUpdateAsync<T>(IEnumerable<T> entities) =>
            _uploader.UpdateAsync(entities);

        async Task IBaseRepository.ExecuteMigrationAndSeedingAsync(IWebHostEnvironment env)
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

        DbSet<T> IBaseRepository.Set<T>() =>
            Set<T>();

        EntityEntry<T> IBaseRepository.Entry<T>(T entity) =>
            base.Entry(entity);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Consts.Db.Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.HasPostgresExtension("pg_trgm");
        }
    }
}