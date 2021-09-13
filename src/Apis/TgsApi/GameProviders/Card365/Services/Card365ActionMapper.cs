using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365ActionMapper
    {
        BalanceResponse Response(GetBalanceResult r);
    }

    public class Card365ActionMapper : ICard365ActionMapper
    {
        public BalanceResponse Response(GetBalanceResult r)
            => new BalanceResponse
            {
                ErrorCode = "200",
                Balance = r.Balance
            };
    }
}