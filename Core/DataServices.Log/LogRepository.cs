using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using EFCore.BulkExtensions;
using TGS.Core.DataServices.Log.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

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

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level)
        {
            return Database.BeginTransactionAsync(level);
        }

        Task<bool> IBaseRepository.CanConnectionAsync()
        {
            return Database.CanConnectAsync();
        }

        IDbConnection IBaseRepository.GetDbConnection()
        {
            return Database.GetDbConnection();
        }

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        Task ILogRepository.AddApiLogsAsync(IList<ApiLog> logs)
        {
            return this.BulkInsertAsync(logs);
        }

        async Task ILogRepository.AddApiLogAsync(ApiLog log)
        {
            await ApiLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        bool ILogRepository.HasDatabaseConnection()
        {
            return Database.CanConnect();
        }

        Task ILogRepository.AddLogRecordsAsync(IList<LogRecord> logs)
        {
            return this.BulkInsertAsync(logs);
        }

        async Task ILogRepository.AddLogRecordAsync(LogRecord log)
        {
            await LogRecords.AddAsync(log);
            await SaveChangesAsync();
        }
        Task IBaseRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        Task ILogRepository.AddAdminLogsAsync(IList<AdminLog> logs)
        {
            return this.BulkInsertAsync(logs);
        }

        async Task ILogRepository.AddAdminLogAsync(AdminLog log)
        {
            await AdminLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
        {
            return this.BulkInsertAsync(entities, bulkConfig, progress);
        }

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
        {
            return this.BulkInsertOrUpdateAsync(entities, bulkConfig, progress);
        }

        Task IBaseRepository.ExecuteMigrationAndSeedingAsync(bool addSeeding)
        {
            var isMigrate = !Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                           Database.GetPendingMigrations().Any();

            if (isMigrate)
            {
                return Database.MigrateAsync();
            }

            return Task.CompletedTask;
        }
    }
}