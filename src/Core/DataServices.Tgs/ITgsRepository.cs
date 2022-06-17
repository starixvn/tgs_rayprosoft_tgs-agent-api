using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs;

public interface ITgsRepository : IBaseRepository
{
    DbSet<AdminLog> AdminLogs { get; set; }

    DbSet<AgentBankInfo> AgentBankInfos { get; set; }

    DbSet<AgentBank> AgentBanks { get; set; }

    DbSet<AgentDividendMultipleLayer> AgentDividendMultipleLayers { get; set; }

    DbSet<AgentDividend> AgentDividends { get; set; }

    DbSet<AgentDomain> AgentDomains { get; set; }

    DbSet<AgentLoginRecord> AgentLoginRecords { get; set; }

    DbSet<Agent> Agents { get; set; }

    DbSet<AgentTransaction> AgentTransactions { get; set; }

    DbSet<AgentWallet> AgentWallets { get; set; }

    DbSet<Announcement> Announcements { get; set; }

    DbSet<ApiLog> ApiLogs { get; set; }

    DbSet<Banner> Banners { get; set; }

    DbSet<BetLimit> BetLimits { get; set; }

    DbSet<BrandAgentDividendCondition> BrandAgentDividendConditions { get; set; }

    DbSet<BrandAllAgentGameTypeCondition> BrandAllAgentGameTypeConditions { get; set; }

    DbSet<BrandAllAgentGameType> BrandAllAgentGameTypes { get; set; }

    DbSet<BrandAllAgent> BrandAllAgents { get; set; }

    DbSet<BrandBetLimit> BrandBetLimits { get; set; }

    DbSet<BrandCommunication> BrandCommunications { get; set; }

    DbSet<BrandCurrency> BrandCurrencies { get; set; }

    DbSet<BrandDomain> BrandDomains { get; set; }

    DbSet<BrandGameProvider> BrandGameProviders { get; set; }

    DbSet<BrandGame> BrandGames { get; set; }

    DbSet<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }

    DbSet<BrandGroupGameProviderBetLimit> BrandGroupGameProviderBetLimits { get; set; }

    DbSet<BrandGroupGame> BrandGroupGames { get; set; }

    DbSet<BrandGroup> BrandGroups { get; set; }

    DbSet<BrandLanguage> BrandLanguages { get; set; }

    DbSet<BrandLiveChat> BrandLiveChats { get; set; }

    DbSet<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

    DbSet<BrandPayment> BrandPayments { get; set; }

    DbSet<BrandPaymentVipLevel> BrandPaymentVipLevels { get; set; }

    DbSet<Brand> Brands { get; set; }

    DbSet<Currency> Currencies { get; set; }

    DbSet<Department> Departments { get; set; }

    DbSet<DepositForm> DepositForms { get; set; }

    DbSet<FailedGameRecordOperation> FailedGameRecordOperations { get; set; }

    DbSet<GameActionBatch> GameActionBatches { get; set; }

    DbSet<GameAction> GameActions { get; set; }

    DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }

    DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

    DbSet<GameProviderCurrency> GameProviderCurrencies { get; set; }

    DbSet<GameProviderFundTransferTransaction> GameProviderFundTransferTransactions { get; set; }

    DbSet<GameProviderLanguage> GameProviderLanguages { get; set; }

    DbSet<GameProvider> GameProviders { get; set; }

    DbSet<Game> Games { get; set; }

    DbSet<Language> Languages { get; set; }

    DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }

    DbSet<LicenseeCurrency> LicenseeCurrencies { get; set; }

    DbSet<LicenseeGameProvider> LicenseeGameProviders { get; set; }

    DbSet<LicenseeLanguage> LicenseeLanguages { get; set; }

    DbSet<Licensee> Licensees { get; set; }

    DbSet<LocalizedString> LocalizedStrings { get; set; }

    DbSet<LogRecord> LogRecords { get; set; }

    DbSet<MarketType> MarketTypes { get; set; }

    DbSet<BrandPaymentAuditLog> PaymentAuditLogs { get; set; }

    DbSet<PaymentChannel> PaymentChannels { get; set; }

    DbSet<PaymentMethodChannel> PaymentMethodChannels { get; set; }

    DbSet<PaymentMethod> PaymentMethods { get; set; }

    DbSet<Payment> Payments { get; set; }

    DbSet<PlatformCredit> PlatformCredits { get; set; }

    DbSet<PlatformPayRecord> PlatformPayRecords { get; set; }

    DbSet<PlayerAggregate> PlayerAggregates { get; set; }

    DbSet<PlayerAuditLog> PlayerAuditLogs { get; set; }

    DbSet<PlayerAuthenticationForm> PlayerAuthenticationForms { get; set; }

    DbSet<PlayerBankInfo> PlayerBankInfos { get; set; }

    DbSet<PlayerBucketHistory> PlayerBucketHistories { get; set; }

    DbSet<PlayerBucket> PlayerBuckets { get; set; }

    DbSet<PlayerFavoriteGame> PlayerFavoriteGames { get; set; }

    DbSet<PlayerGameAggregate> PlayerGameAggregates { get; set; }

    DbSet<PlayerGameSetting> PlayerGameSettings { get; set; }

    DbSet<PlayerIdentity> PlayerIdentity { get; set; }

    DbSet<PlayerIpRecord> PlayerIpRecords { get; set; }

    DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }

    DbSet<PlayerParent> PlayerParents { get; set; }

    DbSet<PlayerProperty> PlayerProperties { get; set; }

    DbSet<PlayerRebate> PlayerRebates { get; set; }

    DbSet<Player> Players { get; set; }

    DbSet<PlayerSportsBetLimit> PlayerSportsBetLimits { get; set; }

    DbSet<PromotionBonus> PromotionBonus { get; set; }

    DbSet<PromotionGameProvider> PromotionGameProviders { get; set; }

    DbSet<PromotionPlayer> PromotionPlayers { get; set; }

    DbSet<Promotion> Promotions { get; set; }

    DbSet<PromotionVipLevel> PromotionVipLevels { get; set; }

    DbSet<ResellerBetLimit> ResellerBetLimits { get; set; }

    DbSet<ResellerCurrency> ResellerCurrencies { get; set; }

    DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }

    DbSet<ResellerLanguage> ResellerLanguages { get; set; }

    DbSet<Reseller> Resellers { get; set; }

    DbSet<Role> Roles { get; set; }

    DbSet<RoundAggregate> RoundAggregates { get; set; }

    DbSet<RoundDetail> RoundDetails { get; set; }

    DbSet<RoundOperationRecord> RoundOperationRecords { get; set; }

    DbSet<Round> Rounds { get; set; }

    DbSet<Setting> Settings { get; set; }

    DbSet<SportMarketType> SportMarketTypes { get; set; }

    DbSet<SportType> SportTypes { get; set; }

    DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }

    DbSet<StationLetter> StationLetters { get; set; }

    DbSet<SubGameProvider> SubGameProviders { get; set; }

    DbSet<SystemLocalization> SystemLocalizations { get; set; }

    DbSet<TransactionPromotion> TransactionPromotions { get; set; }

    DbSet<Transaction> Transactions { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<VipLevel> VipLevels { get; set; }

    DbSet<Wallet> Wallets { get; set; }

    DbSet<Whitelist> Whitelists { get; set; }

    DbSet<WithdrawForm> WithdrawForms { get; set; }
}
