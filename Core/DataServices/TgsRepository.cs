using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.DataInitializers;
using TGS.Core.DataServices.EntityTypeConfigurations;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
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
        DbSet<BrandGame> BrandGames { get; set; }
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
        DbSet<BetLimitGroup> BetLimitGroups { get; set; }
        DbSet<ResellerBetLimit> ResellerBetLimits { get; set; }
        DbSet<LicenseeBetLimit> LicenseeBetLimits { get; set; }
        DbSet<BrandBetLimit> BrandBetLimits { get; set; }
        DbSet<GameProviderBetLimit> GameProviderBetLimits { get; set; }
        DbSet<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        DbSet<PlayerBank> PlayerBanks { get; set; }
        DbSet<VipLevel> VipLevels { get; set; }
        DbSet<PlayerAuditLog> AuditLogs { get; set; }
        DbSet<Announcement> Announcements { get; set; }
        DbSet<StationLetterPlayer> StationLetterPlayers { get; set; }
        DbSet<StationLetter> StationLetters { get; set; }
        DbSet<PaymentVendor> PaymentVendors { get; set; }
        DbSet<PaymentMethod> PaymentMethods { get; set; }
        DbSet<PaymentChannel> PaymentChannels { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<PaymentPlayerList> PaymentPlayerList { get; set; }
        DbSet<PaymentVipLevel> PaymentVipLevels { get; set; }
        DbSet<PaymentAuditLog> PaymentAuditLogs { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Department> Departments { get; set; }
    }

    public class TgsRepository : DbContext, ITgsRepository
    {
        public TgsRepository(DbContextOptions<TgsRepository> options) : base(options)
        {
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
        DbSet<BetLimitGroup> ITgsRepository.BetLimitGroups { get; set; }
        DbSet<LicenseeBetLimit> ITgsRepository.LicenseeBetLimits { get; set; }
        DbSet<GameProviderBetLimit> ITgsRepository.GameProviderBetLimits { get; set; }
        DbSet<PlayerLoginRecord> ITgsRepository.PlayerLoginRecords { get; set; }
        DbSet<PlayerBank> ITgsRepository.PlayerBanks { get; set; }
        DbSet<VipLevel> ITgsRepository.VipLevels { get; set; }
        DbSet<PlayerAuditLog> ITgsRepository.AuditLogs { get; set; }
        DbSet<Announcement> ITgsRepository.Announcements { get; set; }
        DbSet<StationLetterPlayer> ITgsRepository.StationLetterPlayers { get; set; }
        DbSet<StationLetter> ITgsRepository.StationLetters { get; set; }
        DbSet<PaymentVendor> ITgsRepository.PaymentVendors { get; set; }
        DbSet<PaymentMethod> ITgsRepository.PaymentMethods { get; set; }
        DbSet<PaymentChannel> ITgsRepository.PaymentChannels { get; set; }
        DbSet<Payment> ITgsRepository.Payments { get; set; }
        DbSet<PaymentPlayerList> ITgsRepository.PaymentPlayerList { get; set; }
        DbSet<PaymentVipLevel> ITgsRepository.PaymentVipLevels { get; set; }
        DbSet<PaymentAuditLog> ITgsRepository.PaymentAuditLogs { get; set; }
        DbSet<Brand> ITgsRepository.Brands { get; set; }
        DbSet<BrandCurrency> ITgsRepository.BrandCurrencies { get; set; }
        DbSet<BrandLanguage> ITgsRepository.BrandLanguages { get; set; }
        DbSet<ResellerBetLimit> ITgsRepository.ResellerBetLimits { get; set; }
        DbSet<BrandBetLimit> ITgsRepository.BrandBetLimits { get; set; }
        DbSet<User> ITgsRepository.Users { get; set; }
        DbSet<Role> ITgsRepository.Roles { get; set; }
        DbSet<Department> ITgsRepository.Departments { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

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

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
        {
            return this.BulkInsertAsync(entities, bulkConfig, progress);
        }

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig,
            Action<decimal> progress)
        {
            return this.BulkInsertOrUpdateAsync(entities, bulkConfig, progress);
        }

        async Task IBaseRepository.ExecuteMigrationAndSeedingAsync(bool addSeeding)
        {
            var isMigrate = !await Database.GetService<IRelationalDatabaseCreator>().ExistsAsync() ||
                            (await Database.GetPendingMigrationsAsync()).Any();

            if (isMigrate)
            {
                await Database.MigrateAsync();
            }

            if (addSeeding)
            {
                IDataInitializer data = new DataInitializer(this);
                await data.SetInitialDataAsync();
            }
        }

        DbSet<T> IBaseRepository.Set<T>()
        {
            return Set<T>();
        }

        EntityEntry<T> IBaseRepository.Entry<T>(T entity)
        {
            return base.Entry(entity);
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
            modelBuilder.ApplyConfiguration(new ResellerCurrencyMap());
            modelBuilder.ApplyConfiguration(new ResellerLanguageMap());
            modelBuilder.ApplyConfiguration(new ResellerGameProviderMap());

            modelBuilder.ApplyConfiguration(new LicenseeMap());
            modelBuilder.ApplyConfiguration(new LicenseeCurrencyMap());
            modelBuilder.ApplyConfiguration(new LicenseeLanguageMap());
            modelBuilder.ApplyConfiguration(new LicenseeGameProviderMap());

            modelBuilder.ApplyConfiguration(new BrandMap());
            modelBuilder.ApplyConfiguration(new BrandCurrencyMap());
            modelBuilder.ApplyConfiguration(new BrandLanguageMap());
            modelBuilder.ApplyConfiguration(new BrandGameMap());
            modelBuilder.ApplyConfiguration(new BrandGameProviderMap());

            modelBuilder.ApplyConfiguration(new PlayerMap());

            modelBuilder.ApplyConfiguration(new WhitelistTypeMap());
            modelBuilder.ApplyConfiguration(new WalletTypeMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
            modelBuilder.ApplyConfiguration(new LocalizedStringMap());

            modelBuilder.ApplyConfiguration(new GameProviderCurrencyMap());
            modelBuilder.ApplyConfiguration(new GameProviderLanguageMap());

            modelBuilder.ApplyConfiguration(new BetLimitGroupMap());
            modelBuilder.ApplyConfiguration(new ResellerBetLimitMap());
            modelBuilder.ApplyConfiguration(new LicenseeBetLimitMap());
            modelBuilder.ApplyConfiguration(new BrandBetLimitMap());
            modelBuilder.ApplyConfiguration(new GameProviderBetLimitMap());

            modelBuilder.ApplyConfiguration(new PlayerLoginRecordMap());
            modelBuilder.ApplyConfiguration(new PlayerBankMap());
            modelBuilder.ApplyConfiguration(new VipLevelMap());
            modelBuilder.ApplyConfiguration(new AuditLogMap());

            modelBuilder.ApplyConfiguration(new AnnouncementMap());
            modelBuilder.ApplyConfiguration(new StationLetterPlayerMap());
            modelBuilder.ApplyConfiguration(new StationLetterMap());

            modelBuilder.ApplyConfiguration(new PaymentMap());
            modelBuilder.ApplyConfiguration(new PaymentVendorMap());
            modelBuilder.ApplyConfiguration(new PaymentMethodMap());
            modelBuilder.ApplyConfiguration(new PaymentChannelMap());
            modelBuilder.ApplyConfiguration(new PaymentPlayerListMap());
            modelBuilder.ApplyConfiguration(new PaymentVipLevelMap());
            modelBuilder.ApplyConfiguration(new PaymentAuditLogMap());

            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new DepartmentMap());
        }
    }
}