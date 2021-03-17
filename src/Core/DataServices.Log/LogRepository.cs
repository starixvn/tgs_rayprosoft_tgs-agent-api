using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.Bulk;
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

    public sealed class LogRepository : DbContext, ILogRepository
    {
        private readonly NpgsqlBulkUploader _uploader;
        private readonly IBaseRepository _repository;

        public LogRepository(DbContextOptions<LogRepository> options) : base(options)
        {
            _uploader = new(this);
            _repository = this;
        }

        private DbSet<ApiLog> ApiLogs { get; }

        private DbSet<LogRecord> LogRecords { get; }

        private DbSet<AdminLog> AdminLogs { get; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        ChangeTracker IBaseRepository.ChangeTracker => ChangeTracker;

        Task<bool> IBaseRepository.CanConnectionAsync() =>
            Database.CanConnectAsync();

        IDbConnection IBaseRepository.GetDbConnection() =>
            Database.GetDbConnection();

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task ILogRepository.AddApiLogsAsync(IList<ApiLog> logs) =>
            _repository.BulkInsertAsync(logs);

        async Task ILogRepository.AddApiLogAsync(ApiLog log)
        {
            await ApiLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        bool ILogRepository.HasDatabaseConnection() =>
            Database.CanConnect();

        Task ILogRepository.AddLogRecordsAsync(IList<LogRecord> logs) =>
            _repository.BulkInsertAsync(logs);

        async Task ILogRepository.AddLogRecordAsync(LogRecord log)
        {
            await LogRecords.AddAsync(log);
            await SaveChangesAsync();
        }

        Task<int> IBaseRepository.SaveChangesAsync(CancellationToken cancellationToken) =>
            SaveChangesAsync(cancellationToken);

        Task ILogRepository.AddAdminLogsAsync(IList<AdminLog> logs) =>
            _repository.BulkInsertAsync(logs);

        async Task ILogRepository.AddAdminLogAsync(AdminLog log)
        {
            await AdminLogs.AddAsync(log);
            await SaveChangesAsync();
        }

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
        }

        DbSet<T> IBaseRepository.Set<T>() =>
            Set<T>();

        EntityEntry<T> IBaseRepository.Entry<T>(T entity) =>
            base.Entry(entity);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}