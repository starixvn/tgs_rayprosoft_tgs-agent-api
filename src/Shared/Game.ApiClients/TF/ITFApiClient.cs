using System;
using System.Threading.Tasks;

namespace TGS.Shared.Game.ApiClients.TF
{
    public interface ITFApiClient
    {
        ValueTask<string> GetGameUrlAsync(Uri apiUrl, string publicToken, string playerCode);
    }
}
