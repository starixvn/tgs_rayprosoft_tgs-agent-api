using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface IGameProvider
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            IGameProviderConfiguration[] GameProviderConfigurations { get; }
            ICurrency[] Currencies { get; }
            ILanguage[] Languages { get; }
            IGame[] Games { get; }
            bool AutoClosedRound { get; }
            bool SupportTestPlayer { get; }
        }

        internal interface IGameProviderConfiguration
        {
            Guid Id { get; }
            string Name { get; }
            string SecretKey { get; }
            string DesktopEndpoint { get; }            
            string MobileEndpoint { get; }
            string ConfigurationData { get; }
        }

        internal interface IGame
        {
            Guid Id { get; }
            string Name { get; }
            string Code { get; }
            string ExternalId { get; }
            string EndpointPath { get; }
            string GameRuleUrl { get; }
            string Description { get; }
            GameType GameTypeCode { get; }
            PlatformType PlatformType { get; }
            bool HasTurnoverOnTieBet { get; }
            bool SubGame { get; }
        }
    }
}