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
    public class TgsRepository : DbContext, ITgsRepository
    {
        public DbSet<AdminLog> AdminLogs { get; set; }

        public DbSet<AgentBankInfo> AgentBankInfos { get; set; }

        public DbSet<AgentBank> AgentBanks { get; set; }

        public DbSet<AgentDividend> AgentDividends { get; set; }

        public DbSet<AgentDomain> AgentDomains { get; set; }

        public DbSet<AgentLoginRecord> AgentLoginRecord { get; set; }

        public DbSet<AgentLoginRecord> AgentLoginRecords { get; set; }

        public DbSet<Agent> Agents { get; set; }

        public DbSet<AgentTransaction> AgentTransactions { get; set; }

        public DbSet<AgentWallet> AgentWallets { get; set; }

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<ApiLog> ApiLogs { get; set; }

        public DbSet<Banner> Banners { get; set; }

        public DbSet<BetLimit> BetLimits { get; set; }

        public DbSet<BrandBetLimit> BrandBetLimits { get; set; }

        public DbSet<BrandCommunication> BrandCommunications { get; set; }

        public DbSet<BrandCurrency> BrandCurrencies { get; set; }

        public DbSet<BrandDomain> BrandDomains { get; set; }

        public DbSet<BrandGameProvider> BrandGameProviders { get; set; }

        public DbSet<BrandGame> BrandGames { get; set; }

        public DbSet<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }

        public DbSet<BrandGroupGameProviderBetLimit> BrandGroupGameProviderBetLimits { get; set; }

        public DbSet<BrandGroupGame> BrandGroupGames { get; set; }

        public DbSet<BrandGroup> BrandGroups { get; set; }

        public DbSet<BrandLanguage> BrandLanguages { get; set; }

        public DbSet<BrandLiveChat> BrandLiveChats { get; set; }

        public DbSet<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

        public DbSet<BrandPayment> BrandPayments { get; set; }

        public DbSet<BrandPaymentVipLevel> BrandPaymentVipLevels { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public IDbContextTransaction CurrentTransaction =>
            Database.CurrentTransaction;

        public DbSet<Department> Departments { get; set; }

        public DbSet<DepositForm> DepositForms { get; set; }

        public DbSet<FailedGameRecordOperation> FailedGameRecordOperations { get; set; }

        public DbSet<GameActionBatch> GameActionBatches { get; set; }

        public DbSet<GameAction> GameActions { get; set; }

        public DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }

        public DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

        public DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }

        public DbSet<GameProviderFundTransferTransaction> GameProviderFundTransferTransactions { get; set; }

        public DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }

        public DbSet<GameProvider> GameProviders { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }

        public DbSet<LicenseeCurrency> LicenseeCurrencies { get; set; }

        public DbSet<LicenseeGameProvider> LicenseeGameProviders { get; set; }

        public DbSet<LicenseeLanguage> LicenseeLanguages { get; set; }

        public DbSet<Licensee> Licensees { get; set; }

        public DbSet<LocalizedString> LocalizedStrings { get; set; }

        public DbSet<LogRecord> LogRecords { get; set; }

        public DbSet<MarketType> MarketTypes { get; set; }

        public DbSet<BrandPaymentAuditLog> PaymentAuditLogs { get; set; }

        public DbSet<PaymentChannel> PaymentChannels { get; set; }

        public DbSet<PaymentMethodChannel> PaymentMethodChannels { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PlatformCredit> PlatformCredits { get; set; }

        public DbSet<PlatformPayRecord> PlatformPayRecords { get; set; }

        public DbSet<PlayerAggregate> PlayerAggregates { get; set; }

        public DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }

        public DbSet<PlayerBankInfo> PlayerBankInfos { get; set; }

        public DbSet<PlayerBucketHistory> PlayerBucketHistories { get; set; }

        public DbSet<PlayerBucket> PlayerBuckets { get; set; }

        public DbSet<PlayerFavoriteGame> PlayerFavoriteGames { get; set; }

        public DbSet<PlayerGameAggregate> PlayerGameAggregates { get; set; }

        public DbSet<PlayerGameSetting> PlayerGameSettings { get; set; }

        public DbSet<PlayerIpRecord> PlayerIpRecords { get; set; }

        public DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }

        public DbSet<PlayerRebate> PlayerRebates { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerSportsBetLimit> PlayerSportsBetLimits { get; set; }

        public DbSet<PromotionBonus> PromotionBonus { get; set; }

        public DbSet<PromotionGameProvider> PromotionGameProviders { get; set; }

        public DbSet<PromotionPlayer> PromotionPlayers { get; set; }

        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<PromotionVipLevel> PromotionVipLevels { get; set; }

        public DbSet<ResellerBetLimit> ResellerBetLimits { get; set; }

        public DbSet<ResellerCurrency> ResellerCurrencies { get; set; }

        public DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }

        public DbSet<ResellerLanguage> ResellerLanguages { get; set; }

        public DbSet<Reseller> Resellers { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RoundAggregate> RoundAggregates { get; set; }

        public DbSet<RoundDetail> RoundDetails { get; set; }

        public DbSet<RoundOperationRecord> RoundOperationRecords { get; set; }

        public DbSet<Round> Rounds { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<SportMarketType> SportMarketTypes { get; set; }

        public DbSet<SportType> SportTypes { get; set; }

        public DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }

        public DbSet<StationLetter> StationLetters { get; set; }

        public DbSet<SubGameProvider> SubGameProviders { get; set; }

        public DbSet<SystemLocalization> SystemLocalizations { get; set; }

        public DbSet<TransactionPromotion> TransactionPromotions { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<VipLevel> VipLevels { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Whitelist> Whitelists { get; set; }

        public DbSet<WithdrawForm> WithdrawForms { get; set; }

        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        public Task<bool> CanConnectionAsync() => Database.CanConnectAsync();

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

        public IDbConnection GetDbConnection() =>
            Database.GetDbConnection();

        public void SetTimeoutToSeconds(int seconds) => Database.SetCommandTimeout(seconds);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Consts.Db.Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.HasPostgresExtension("pg_trgm");
        }
    }
}
