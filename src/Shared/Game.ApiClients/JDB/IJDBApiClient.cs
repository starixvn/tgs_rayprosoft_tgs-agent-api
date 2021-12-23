using System;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings.JDB;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public interface IJDBApiClient
    {
        ValueTask<BetHistoryResponse> GetBetHistoryUrlAsync(Uri apiUrl, JDBAgent agent, BetHistoryRequest request);

        ValueTask<GetTokenResponse> GetTokenAsync(Uri apiUrl, JDBAgent agent, GetTokenRequest request);
    }
}
