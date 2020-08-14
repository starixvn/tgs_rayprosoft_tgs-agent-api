using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.EntityTypeConfiguration;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
{
    public class TgsRepository : DbContext, ITgsRepository
    {
        DbSet<Reseller> ITgsRepository.Resellers { get; set; }
        DbSet<Game> ITgsRepository.Games { get; set; }
        DbSet<GameProvider> ITgsRepository.GameProviders { get; set; }
        DbSet<GameProviderConfiguration> ITgsRepository.GameProviderConfigurations { get; set; }
        DbSet<ResellerGameProvider> ITgsRepository.ResellerGameProviders { get; set; }
        DbSet<Currency> ITgsRepository.Currencies { get; set; }
        DbSet<Language> ITgsRepository.Languages { get; set; }

        public TgsRepository(DbContextOptions options) : base(options)
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

            modelBuilder.ApplyConfiguration(new CurrencyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GameProviderTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GpcTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResellerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResellerGameProviderTypeConfiguration());
        }
    }
}
