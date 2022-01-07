using TGS.Core.Models.GameProviders.Settings.WMCasino;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoAgentMapper
    {
        WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode);

        WMCasinoAgent GetAgentBySignature(WMCasinoBrandMerchant[] merchants, string signature);

        WMCasinoAgent GetAgentById(WMCasinoBrandMerchant[] merchants, string agentId);
    }
}