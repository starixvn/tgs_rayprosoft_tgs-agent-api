using System;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.LY.Commons;
using TGS.Shared.Game.Models.LY.Enums;
using TGS.Shared.Game.Models.LY.Outgoing;

namespace TGS.Shared.Game.ApiClients.LY
{
    public interface ILYApiClient
    {
        ValueTask<LYApiResponse<LYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYAction action, LYFnudRequest request);

        ValueTask<LYApiResponse<LYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYFundCheckingRequest request);

        ValueTask<LYApiResponse<LYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYGetBalanceRequest request);

        ValueTask<LYApiResponse<LYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameUrlRequest request);
    }
}
