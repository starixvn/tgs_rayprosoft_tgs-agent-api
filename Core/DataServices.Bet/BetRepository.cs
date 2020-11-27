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
using TGS.Core.DataServices.Bet.EntityTypeConfigurations;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet
{
    public interface IBetRepository : IBaseRepository
    {
        DbSet<Round> Rounds { get; set; }

        DbSet<RoundDetail> RoundDetails { get; set; }

        DbSet<GameAction> GameActions { get; set; }

        DbSet<GameActionBatch> GameActionBatches { get; set; }
    }

    public class BetRepository : DbContext, IBetRepository
    {
        DbSet<Round> IBetRepository.Rounds { get; set; }
        DbSet<RoundDetail> IBetRepository.RoundDetails { get; set; }
        DbSet<GameAction> IBetRepository.GameActions { get; set; }
        DbSet<GameActionBatch> IBetRepository.GameActionBatches { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

        public BetRepository(DbContextOptions<BetRepository> options) : base(options)
        {

        }

        Task<bool> IBaseRepository.CanConnectionAsync() => 
            Database.CanConnectAsync();

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) => 
            Database.BeginTransactionAsync(level);

        IDbConnection IBaseRepository.GetDbConnection() => 
            Database.GetDbConnection();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new RoundMap());
            modelBuilder.ApplyConfiguration(new RoundDetailMap());
            modelBuilder.ApplyConfiguration(new GameActionMap());
            modelBuilder.ApplyConfiguration(new GameActionBatchMap());
        }

        Task IBaseRepository.SaveChangesAsync() => 
            SaveChangesAsync();

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress) => 
            this.BulkInsertAsync(entities, bulkConfig, progress);

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress) => 
            this.BulkInsertOrUpdateAsync(entities, bulkConfig, progress);

        async Task IBaseRepository.ExecuteMigrationAndSeedingAsync(bool addSeeding)
        {
            var isMigrate = !await Database.GetService<IRelationalDatabaseCreator>().ExistsAsync() ||
                (await Database.GetPendingMigrationsAsync()).Any();

            if (isMigrate)
            {
                await Database.MigrateAsync();
            }
        }

        DbSet<T> IBaseRepository.Set<T>() => 
            Set<T>();

        EntityEntry<T> IBaseRepository.Entry<T>(T entity) => 
            base.Entry(entity);
    }
}
