using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices.Bet.EntityTypeConfigurations;
using TGS.Core.DataServices.Bet.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.DataServices.Bet
{
    public class BetRepository : DbContext, IBetRepository
    {
        DbSet<Round> IBetRepository.Rounds { get; set; }
        DbSet<RoundDetail> IBetRepository.RoundDetails { get; set; }
        DbSet<GameAction> IBetRepository.GameActions { get; set; }
        DbSet<GameActionBatch> IBetRepository.GameActionBatches { get; set; }

        public BetRepository(DbContextOptions<BetRepository> options) : base(options)
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

        DbConnection IBaseRepository.GetDbConnection()
        {
            return Database.GetDbConnection();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new RoundTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoundDetailTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameActionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameActionBatchTypeConfiguration());
        }
    }
}
