using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log
{
    public interface ILogRepository : IBaseRepository
    {
        Task AddApiLogsAsync(IList<ApiLog> logs);

        Task AddApiLogAsync(ApiLog log);

        Task AddLogRecordsAsync(IList<LogRecord> logs);

        Task AddLogRecordAsync(LogRecord log);

        Task AddAdminLogsAsync(IList<AdminLog> logs);

        Task AddAdminLogAsync(AdminLog log);

        bool HasDatabaseConnection();
    }

    public class LogRepository : DbContext, ILogRepository
    {
        public LogRepository(DbContextOptions<LogRepository> options) : base(options)
        {
        }

        private DbSet<ApiLog> ApiLogs { get; }

        private DbSet<LogRecord> LogRecords { get; }

        private DbSet<AdminLog> AdminLogs { get; }

        IDbContextTransaction IBaseRepository.CurrentTransaction => 
            Database.CurrentTransaction;

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) => 
            Database.BeginTransactionAsync(level);

        Task<bool> IBaseRepository.CanConnectionAsync() => 
            Database.CanConnectAsync();

        IDbConnection IBaseRepository.GetDbConnection() => 
            Database.GetDbConnection();

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task ILogRepository.AddApiLogsAsync(IList<ApiLog> logs) => 
            this.BulkInsertAsync(logs);

        async Task ILogRepository.AddApiLogAsync(ApiLog log)
        {
            await ApiLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        bool ILogRepository.HasDatabaseConnection() => 
            Database.CanConnect();

        Task ILogRepository.AddLogRecordsAsync(IList<LogRecord> logs) => 
            this.BulkInsertAsync(logs);

        async Task ILogRepository.AddLogRecordAsync(LogRecord log)
        {
            await LogRecords.AddAsync(log);
            await SaveChangesAsync();
        }

        Task IBaseRepository.SaveChangesAsync() => 
            SaveChangesAsync();

        Task ILogRepository.AddAdminLogsAsync(IList<AdminLog> logs) => 
            this.BulkInsertAsync(logs);

        async Task ILogRepository.AddAdminLogAsync(AdminLog log)
        {
            await AdminLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress) => 
            this.BulkInsertAsync(entities, bulkConfig, progress);

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig,
            Action<decimal> progress) =>
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        DbSet<T> IBaseRepository.Set<T>() => 
            Set<T>();

        EntityEntry<T> IBaseRepository.Entry<T>(T entity) =>
            base.Entry(entity);
    }
}