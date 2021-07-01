using System;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings.AllBet;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetAgentMapper
    {
        AllBetAgent GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode);
    }

    public class AllBetAgentMapper : IAllBetAgentMapper
    {
        private static readonly string _DefaultCode = "Default";

        AllBetAgent IAllBetAgentMapper.GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode)
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

            return merchant.Agents.FirstOrDefault(a => a.CurrencyCode == currencyCode);
        }
    }
}