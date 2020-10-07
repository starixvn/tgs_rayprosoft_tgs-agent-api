using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOGameControllerMapper
    {
        GetBalanceResponse Response(string playerCode, GetBalanceResult r);
    }

    public sealed class SBOGameControllerMapper : ISBOGameControllerMapper
    {
        GetBalanceResponse ISBOGameControllerMapper.Response(string playerCode, GetBalanceResult r)
        {
            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.Description(),
                PlayerCode = playerCode,
                Balance = r.Balance
            };
        }
    }
}
