using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TGS.Core.DataService.Models;

namespace TGS.Core.DataService
{
    public interface ITgsRepository
    {

        DbSet<Game> Games { get; set; }

        DbSet<GameProvider> GameProviders { get; set; }

        DbSet<GameProviderConfiguration> GameProviderConfigurations { get; set; }

        Task<bool> CanConnectionAsync();
        void SetTimeoutToSeconds(int seconds);
    }
}
