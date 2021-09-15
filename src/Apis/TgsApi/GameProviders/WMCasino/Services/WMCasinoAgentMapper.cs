using System;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings.WMCasino;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoAgentMapper
    {
        WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode);

        WMCasinoAgent GetAgentBySignature(WMCasinoBrandMerchant[] merchants, string signature);

        WMCasinoAgent GetAgentById(WMCasinoBrandMerchant[] merchants, string agentId);
    }

    public class WMCasinoAgentMapper : IWMCasinoAgentMapper
    {
        private static readonly string _DefaultCode = "Default";

        public WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode)
        {
            if (merchants == null || merchants.Length == 0)
            {
                throw new ArgumentException("Brand Merchant setting is empty");
            }

            var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                merchants.FirstOrDefault(m => m.BrandCode == _DefaultCode);

            if (merchant == null)
            {
                throw new ArgumentException($"No Merchant of '{brandCode}' is found.");
            }

            return merchant.Agents.FirstOrDefault(ag => ag.CurrencyCode == currencyCode && ag.Status);
        }

        public WMCasinoAgent GetAgentById(WMCasinoBrandMerchant[] merchants, string agentId)
            => merchants
                .SelectMany(m => m.Agents)
                .FirstOrDefault(ag => ag.AgentId.Equals(agentId) && ag.Status);

        public WMCasinoAgent GetAgentBySignature(WMCasinoBrandMerchant[] merchants, string signature)
            => merchants
                .SelectMany(m => m.Agents)
                .FirstOrDefault(ag => ag.Signature.Equals(signature) && ag.Status);
    }
}