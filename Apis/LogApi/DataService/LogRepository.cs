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

        DbSet<ApiLog> ApiLogs { get; set; }

        DbSet<LogRecord> LogRecords { get; set; }

        DbSet<AdminLog> AdminLogs { get; set; }

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
            modelBuilder.ApplyConfiguration(new ApiLogMap());
            modelBuilder.ApplyConfiguration(new LogRecordMap());
            modelBuilder.ApplyConfiguration(new ServiceLogMap());
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
        Task IBaseRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        async Task ILogRepository.AddAdminLogsAsync(IList<AdminLog> logs)
        {
            await AdminLogs.AddRangeAsync(logs);
            await SaveChangesAsync();
        }

        async Task ILogRepository.AddAdminLogAsync(AdminLog log)
        {
            await AdminLogs.AddAsync(log);
            await SaveChangesAsync();
        }
    }
}