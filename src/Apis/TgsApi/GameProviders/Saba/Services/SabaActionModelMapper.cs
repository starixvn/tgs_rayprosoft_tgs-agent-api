using System;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public interface ISabaActionModelMapper
    {
        GetBalanceResponse Response(string playerCode, GetBalanceResult result);
    }

    public sealed class SabaActionModelMapper : ISabaActionModelMapper
    {
        private readonly TimeSpan _timeSpan = TimeSpan.FromHours(-4);

        GetBalanceResponse ISabaActionModelMapper.Response(string playerCode, GetBalanceResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                PlayerCode = playerCode,
                Balance = result.Balance,
                Timestamp = DateTimeOffset.Now.ToOffset(_timeSpan).ToIso8601String(true)
            };
    }
}