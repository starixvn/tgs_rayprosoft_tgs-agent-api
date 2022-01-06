using System;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public interface IJDBApiClient
    {
        ValueTask<BetHistoryResponse> GetBetHistoryUrlAsync(
            Uri apiUrl,
            string clientDomain,
            string aesIv,
            string aesKey,
            BetHistoryRequest request);

        ValueTask<GetTokenResponse> GetTokenAsync(
            Uri apiUrl,
            string clientDomain,
            string aesIv,
            string aesKey,
            GetTokenRequest request);
    }
}
