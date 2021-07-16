using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

        DbSet<PlayerBankInfo> PlayerBankInfos { get; set; }

        DbSet<VipLevel> VipLevels { get; set; }

        DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }

        DbSet<Announcement> Announcements { get; set; }

        DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }

        DbSet<StationLetter> StationLetters { get; set; }

        DbSet<Payment> Payments { get; set; }

        DbSet<PaymentMethod> PaymentMethods { get; set; }

        DbSet<PaymentChannel> PaymentChannels { get; set; }

        DbSet<PaymentMethodChannel> PaymentMethodChannels { get; set; }

        DbSet<BrandPayment> BrandPayments { get; set; }

        DbSet<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

        DbSet<BrandPaymentVipLevel> BrandPaymentVipLevels { get; set; }

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

        DbSet<Agent> Agents { get; set; }

        DbSet<AgentBank> AgentBanks { get; set; }

        DbSet<AgentBankInfo> AgentBankInfos { get; set; }

        DbSet<AgentWallet> AgentWallets { get; set; }

        DbSet<AgentTransaction> AgentTransactions { get; set; }

        DbSet<Promotion> Promotions { get; set; }

        DbSet<PromotionGameProvider> PromotionGameProviders { get; set; }

        DbSet<PromotionVipLevel> PromotionVipLevels { get; set; }

        DbSet<DepositForm> DepositForms { get; set; }

        DbSet<WithdrawForm> WithdrawForms { get; set; }

        DbSet<TransactionPromotion> TransactionPromotions { get; set; }

        DbSet<PlayerBucket> PlayerBuckets { get; set; }
        DbSet<PromotionBonus> PromotionBonus { get; set; }
        DbSet<PromotionPlayer> PromotionPlayers { get; set; }
        DbSet<AgentDividend> AgentDividends { get; set; }
        DbSet<PlayerRebate> PlayerRebates { get; set; }
        DbSet<AgentLoginRecord> AgentLoginRecords { get; set; }
        DbSet<BrandDomain> BrandDomains { get; set; }
        DbSet<GameProviderFundTransferTransaction> GameProviderFundTransferTransactions { get; set; }
        DbSet<FailedGameRecordOperation> FailedGameRecordOperations { get; set; }
        DbSet<RoundAggregate> RoundAggregates { get; set; }
        DbSet<PlayerAggregate> PlayerAggregates { get; set; }
        DbSet<PlayerGameAggregate> PlayerGameAggregates { get; set; }
        DbSet<PlayerBucketHistory> PlayerBucketHistories { get; set; }
        DbSet<PlatformCredit> PlatformCredits { get; set; }
        DbSet<PlatformPayRecord> PlatformPayRecords { get; set; }

        DbSet<ApiLog> ApiLogs { get; set; }

        DbSet<LogRecord> LogRecords { get; set; }

        DbSet<AdminLog> AdminLogs { get; set; }
    }

    public class TgsRepository : DbContext, ITgsRepository
    {
        public DbSet<AgentLoginRecord> AgentLoginRecord { get; set; }

        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {
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

        public DbSet<PlayerBankInfo> PlayerBankInfos { get; set; }

        public DbSet<VipLevel> VipLevels { get; set; }

        public DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }

        public DbSet<StationLetter> StationLetters { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<PaymentChannel> PaymentChannels { get; set; }

        public DbSet<BrandPayment> BrandPayments { get; set; }

        public DbSet<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

        public DbSet<BrandPaymentVipLevel> BrandPaymentVipLevels { get; set; }

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

        public DbSet<Agent> Agents { get; set; }

        public DbSet<AgentBank> AgentBanks { get; set; }

        public DbSet<AgentWallet> AgentWallets { get; set; }

        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<PromotionGameProvider> PromotionGameProviders { get; set; }

        public DbSet<PromotionVipLevel> PromotionVipLevels { get; set; }

        public DbSet<AgentTransaction> AgentTransactions { get; set; }

        public DbSet<PaymentMethodChannel> PaymentMethodChannels { get; set; }

        public DbSet<TransactionPromotion> TransactionPromotions { get; set; }

        public DbSet<PlayerBucket> PlayerBuckets { get; set; }
        public DbSet<PromotionBonus> PromotionBonus { get; set; }
        public DbSet<PromotionPlayer> PromotionPlayers { get; set; }
        public DbSet<AgentDividend> AgentDividends { get; set; }
        public DbSet<PlayerRebate> PlayerRebates { get; set; }
        public DbSet<AgentLoginRecord> AgentLoginRecords { get; set; }
        public DbSet<BrandDomain> BrandDomains { get; set; }
        public DbSet<GameProviderFundTransferTransaction> GameProviderFundTransferTransactions { get; set; }

        public DbSet<FailedGameRecordOperation> FailedGameRecordOperations { get; set; }
        public DbSet<RoundAggregate> RoundAggregates { get; set; }
        public DbSet<PlayerAggregate> PlayerAggregates { get; set; }
        public DbSet<PlayerGameAggregate> PlayerGameAggregates { get; set; }
        public DbSet<PlayerBucketHistory> PlayerBucketHistories { get; set; }
        public DbSet<PlatformCredit> PlatformCredits { get; set; }
        public DbSet<PlatformPayRecord> PlatformPayRecords { get; set; }

        public DbSet<ApiLog> ApiLogs { get; set; }

        public DbSet<LogRecord> LogRecords { get; set; }

        public DbSet<AdminLog> AdminLogs { get; set; }

        public IDbContextTransaction CurrentTransaction =>
            Database.CurrentTransaction;

        public DbSet<DepositForm> DepositForms { get; set; }

        public DbSet<WithdrawForm> WithdrawForms { get; set; }
        public DbSet<AgentBankInfo> AgentBankInfos { get; set; }
        public DbSet<BrandCommunication> BrandCommunications { get; set; }

        public Task<bool> CanConnectionAsync() => Database.CanConnectAsync();

        public void SetTimeoutToSeconds(int seconds) => Database.SetCommandTimeout(seconds);

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        public IDbConnection GetDbConnection() =>
            Database.GetDbConnection();

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