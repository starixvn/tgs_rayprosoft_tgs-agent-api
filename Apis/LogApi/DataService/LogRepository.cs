using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Apis.LogApi.DataService.EntityTypeConfiguration;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.LogApi.DataService
{
    public interface ILogRepository : IBaseRepository
    {
        Task AddApiLogsAsync(IList<ApiLog> logs);

        Task AddApiLogAsync(ApiLog log);

        Task AddEventLogsAsync(IList<EventLog> logs);

        Task AddEventLogAsync(EventLog log);

        Task AddLogRecordsAsync(IList<LogRecord> logs);

        Task AddLogRecordAsync(LogRecord log);

        void CreateDbIfNotExist();
        bool HasDatabaseConnection();
    }

    public class LogRepository : DbContext, ILogRepository
    {
        public LogRepository(DbContextOptions<LogRepository> options) : base(options)
        {
        }

        DbSet<ApiLog> ApiLogs { get; set; }

        DbSet<EventLog> EventLogs { get; set; }

        DbSet<LogRecord> LogRecords { get; set; }

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

            modelBuilder.ApplyConfiguration(new EventLogTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ApiLogTypeConfiguration());
        }

        async Task ILogRepository.AddApiLogsAsync(IList<ApiLog> logs)
        {
            await ApiLogs.AddRangeAsync(logs);
            await SaveChangesAsync();
        }

        async Task ILogRepository.AddApiLogAsync(ApiLog log)
        {
            await ApiLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        async Task ILogRepository.AddEventLogsAsync(IList<EventLog> logs)
        {
            await EventLogs.AddRangeAsync(logs);
            await SaveChangesAsync();
        }

        async Task ILogRepository.AddEventLogAsync(EventLog log)
        {
            await EventLogs.AddAsync(log);
            await SaveChangesAsync();
        }

        void ILogRepository.CreateDbIfNotExist()
        {
            throw new System.NotImplementedException();
        }

        bool ILogRepository.HasDatabaseConnection()
        {
            return Database.CanConnect();
        }

        async Task ILogRepository.AddLogRecordsAsync(IList<LogRecord> logs)
        {
            await LogRecords.AddRangeAsync(logs);
            await SaveChangesAsync();
        }

        async Task ILogRepository.AddLogRecordAsync(LogRecord log)
        {

            await LogRecords.AddAsync(log);
            await SaveChangesAsync();
        }
    }
}
