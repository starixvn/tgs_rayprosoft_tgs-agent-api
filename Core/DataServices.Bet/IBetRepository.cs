using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Bet.Models;

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
