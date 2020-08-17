using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Bet.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.DataServices.Bet
{
    public interface IBetRepository : IBaseRepository
    {
        DbSet<Round> Rounds { get; set; }

        DbSet<RoundDetail> RoundDetails { get; set; }

        DbSet<GameAction> GameActions { get; set; }

        DbSet<GameActionBatch> GameActionBatches { get; set; }
    }
}
