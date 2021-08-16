using System;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoActionModelMapper
    {
        GetBalanceResponse Response(string playerCode, GetBalanceResult r);
        WalletTransferResponse Response(WalletTransferRequest request, decimal balance);
    }

    public class WMCasinoActionModelMapper : IWMCasinoActionModelMapper
    {
        private const string _TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private const string _GameProviderCode = Consts.GameProvider.WMCasino;
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        public GetBalanceResponse Response(string playerCode, GetBalanceResult r)
            => new GetBalanceResponse
            {
                ErrorCode = (int)GameApiError.Success,
                Data = new GetBalanceResponse.Result
                {
                    PlayerCode = playerCode,
                    Balance = r.Balance,
                    Timestamp = GetTimestamp()
                }
            };

        public WalletTransferResponse Response(WalletTransferRequest request, decimal balance)
            => new WalletTransferResponse
            {
                ErrorCode = (int)GameApiError.Success,
                Data = new WalletTransferResponse.Result
                {
                    Amount = request.Amount,
                    Balance = balance,
                    DealId = request.DealId,
                    Timestamp = GetTimestamp()
                }
            };

        private static string GetTimestamp()
            => DateTimeOffset.UtcNow.ToOffset(_Offset).ToString(_TimestampFormat);
    }
}