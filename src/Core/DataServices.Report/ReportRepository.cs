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
using TGS.Core.DataServices.Report.Models;
using TGS.Core.Models;

namespace TGS.Core.DataServices.Report
{
    public interface IReportRepository : IBaseRepository
    {
        DbSet<Market> Markets { get; set; }
    }

    public class ReportRepository : DbContext, IReportRepository
    {
        private readonly NpgsqlBulkUploader _uploader;

        public ReportRepository(DbContextOptions<ReportRepository> options) : base(options)
        {
            _uploader = new(this);
        }

        DbSet<Market> IReportRepository.Markets { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

        ChangeTracker IBaseRepository.ChangeTracker => ChangeTracker;

        Task<bool> IBaseRepository.CanConnectionAsync() => Database.CanConnectAsync();

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        IDbConnection IBaseRepository.GetDbConnection() =>
            Database.GetDbConnection();

        Task<int> IBaseRepository.SaveChangesAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

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
            modelBuilder.HasDefaultSchema(Consts.Db.Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}