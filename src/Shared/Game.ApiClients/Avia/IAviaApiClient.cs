using System;
using System.Threading.Tasks;

namespace TGS.Shared.Game.ApiClients.Avia
{
    public interface IAviaApiClient
    {
        ValueTask<(bool IsPlayer, Uri? GameUrl)> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId);

        ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency);
    }
}
