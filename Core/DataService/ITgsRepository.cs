using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices
{
    public interface ITgsRepository
    {
        DbSet<Currency> Currencies { get; set; }

        DbSet<Language> Languages { get; set; }

        DbSet<Reseller> Resellers { get; set; }

        DbSet<ResellerGameProvider> ResellerGameProviders { get; set; }

        DbSet<Game> Games { get; set; }

        DbSet<GameProvider> GameProviders { get; set; }

        DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

        Task<bool> CanConnectionAsync();
        void SetTimeoutToSeconds(int seconds);
    }
}
