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
using TGS.Core.DataServices.Bet.DataInitializers;
using TGS.Core.DataServices.Bet.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;

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
        private readonly NpgsqlBulkUploader _uploader;

        public BetRepository(DbContextOptions<BetRepository> options) : base(options)
        {
            _uploader = new(this);
        }

        DbSet<Round> IBetRepository.Rounds { get; set; }
        DbSet<RoundDetail> IBetRepository.RoundDetails { get; set; }
        DbSet<GameAction> IBetRepository.GameActions { get; set; }
        DbSet<GameActionBatch> IBetRepository.GameActionBatches { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

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

        Task IBaseRepository.SaveChangesAsync() =>
            SaveChangesAsync();

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
                IDataInitializer data = new DataInitializer(this);
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
        }
    }
}