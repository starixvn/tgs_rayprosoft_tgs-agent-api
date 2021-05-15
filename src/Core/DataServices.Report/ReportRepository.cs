using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.Models;

namespace TGS.Core.DataServices.Report
{
    public interface IReportRepository : IBaseRepository
    {
        DbSet<RoundAggregate> RoundAggregates { get; set; }
        DbSet<PlayerAggregate> PlayerAggregates { get; set; }
        DbSet<PlayerGameTypeAggregate> PlayerGameTypeAggregates { get; set; }
    }

    public class ReportRepository : DbContext, IReportRepository
    {
        public ReportRepository(DbContextOptions<ReportRepository> options) : base(options)
        {
        }

        DbSet<RoundAggregate> IReportRepository.RoundAggregates { get; set; }
        DbSet<PlayerAggregate> IReportRepository.PlayerAggregates { get; set; }
        DbSet<PlayerGameTypeAggregate> IReportRepository.PlayerGameTypeAggregates { get; set; }

        IDbContextTransaction IBaseRepository.CurrentTransaction =>
            Database.CurrentTransaction;

        ChangeTracker IBaseRepository.ChangeTracker => ChangeTracker;

        Task<bool> IBaseRepository.CanConnectionAsync() => Database.CanConnectAsync();

        void IBaseRepository.SetTimeoutToSeconds(int seconds) => Database.SetCommandTimeout(seconds);

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level) =>
            Database.BeginTransactionAsync(level);

        IDbConnection IBaseRepository.GetDbConnection() =>
            Database.GetDbConnection();

        Task<int> IBaseRepository.SaveChangesAsync(CancellationToken cancellationToken) =>
            SaveChangesAsync(cancellationToken);

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