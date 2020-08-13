using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TGS.Core.DataService.EntityTypeConfiguration;
using TGS.Core.DataService.Models;

namespace TGS.Core.DataService
{
    public class TgsRepository : DbContext, ITgsRepository
    {
        DbSet<Game> ITgsRepository.Games { get; set; }
        DbSet<GameProvider> ITgsRepository.GameProviders { get; set; }
        DbSet<GameProviderConfiguration> ITgsRepository.GameProviderConfigurations { get; set; }

        public TgsRepository(DbContextOptions options)
        {

        }

        Task<bool> ITgsRepository.CanConnectionAsync()
        {
            return Database.CanConnectAsync();
        }

        void ITgsRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new GameTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameProviderTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GpcTypeConfiguration());
        }
    }
}
