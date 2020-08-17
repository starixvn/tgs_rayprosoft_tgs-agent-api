using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Bet.EntityTypeConfigurations;
using TGS.Core.DataServices.Bet.Models;

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

        Task<bool> IBetRepository.CanConnectionAsync()
        {
            return Database.CanConnectAsync();
        }

        void IBetRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
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
