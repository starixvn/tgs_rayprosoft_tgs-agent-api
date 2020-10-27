using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.DataInitializers;
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
        DbSet<LicenseeGameProvider> ITgsRepository.LicenseeGameProviders { get; set; }
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
        DbSet<PlayerBank> ITgsRepository.PlayerBanks { get; set; }
        DbSet<VipLevel> ITgsRepository.VipLevels { get; set; }
        DbSet<PlayerAuditLog> ITgsRepository.AuditLogs { get; set; }
        DbSet<Announcement> ITgsRepository.Announcements { get; set; }
        DbSet<StationLetterPlayer> ITgsRepository.StationLetterPlayers { get; set; }
        DbSet<StationLetter> ITgsRepository.StationLetters { get; set; }
        
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
            modelBuilder.ApplyConfiguration(new LicenseeGameProviderMap());
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
            modelBuilder.ApplyConfiguration(new PlayerBankMap());
            modelBuilder.ApplyConfiguration(new VipLevelMap());
            modelBuilder.ApplyConfiguration(new AuditLogMap());

            modelBuilder.ApplyConfiguration(new AnnouncementMap());
            modelBuilder.ApplyConfiguration(new StationLetterPlayerMap());
            modelBuilder.ApplyConfiguration(new StationLetterMap());
        }

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
        {
            return this.BulkInsertAsync(entities, bulkConfig, progress);
        }

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
        {
            return this.BulkInsertOrUpdateAsync(entities, bulkConfig, progress);
        }

        async Task IBaseRepository.ExecuteMigrationAndSeedingAsync(bool addSeeding)
        {           
            var isMigrate = !Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                           Database.GetPendingMigrations().Any();

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
    }
}
