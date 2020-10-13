using Microsoft.EntityFrameworkCore;

namespace TGS.Core.DataServices.Bet
{
    public interface IBetNoDetectChangesRepository : IBetRepository
    {
       
    }

    public class BetNoDetectChangesRepository : BetRepository, IBetNoDetectChangesRepository
    {
        public BetNoDetectChangesRepository(DbContextOptions<BetRepository> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled= false;
        }
    }
}
