using System;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetActionModelMapper
    {
        BalanceResponse Response(GetBalanceResult result);
    }

    public class AllBetActionModelMapper : IAllBetActionModelMapper
    {
        BalanceResponse IAllBetActionModelMapper.Response(GetBalanceResult result)
            => GetBalanceResponse(result.Balance);

        private static BalanceResponse GetBalanceResponse(decimal balance)
            => new BalanceResponse
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.ToString(),
                Balance = balance,
                Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
    }
}