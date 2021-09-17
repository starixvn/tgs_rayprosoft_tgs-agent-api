using System;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings.Card365;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365AgentMapper
    {
        public Card365BrandAgent GetAgent(Card365BrandAgent[] brandAgents, string brandCode);
    }

    public class Card365AgentMapper : ICard365AgentMapper
    {
        private static readonly string _DefaultCode = "Default";

        public Card365BrandAgent GetAgent(Card365BrandAgent[] brandAgents, string brandCode)
        {
            if (brandAgents == null || brandAgents.Length == 0)
            {
                throw new ArgumentException("Brand Agent setting is empty");
            }

            return brandAgents.FirstOrDefault(ag => ag.BrandCode == brandCode) ??
                brandAgents.FirstOrDefault(ag => ag.BrandCode == _DefaultCode);
        }
    }
}