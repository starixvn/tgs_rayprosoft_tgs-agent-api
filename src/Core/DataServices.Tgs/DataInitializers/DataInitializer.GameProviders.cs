// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        #region Private Method

        private static IGame[] LoadGamesFromCsv(string gameProviderCode)
        {
            var type = typeof(DataInitializer);
            var @namespace = type.Namespace;

            if (gameProviderCode.IsNullOrEmpty())
            {
                return null;
            }

            var csvName = $"{@namespace}.Games.{gameProviderCode}.csv";
            using var stream = type.Assembly.GetManifestResourceStream(csvName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find game setting file of '{gameProviderCode}'");
            }

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Game>().ToList().Cast<IGame>().ToArray();
        }

        #endregion Private Method

        #region Interfaces and Base Models

        internal interface ICodeMapper
        {
            string Code { get; }

            string ExternalCode { get; }
        }

        internal interface IGame
        {
            string AvailableCurrencies { get; init; }

            bool AvailableToBrand { get; init; }

            string Code { get; init; }

            string Description { get; init; }

            string EndpointPath { get; init; }

            string ExternalId { get; init; }

            string GameRuleUrl { get; init; }

            GameType GameTypeCode { get; init; }

            bool HasTurnoverOnTieBet { get; init; }

            bool Html5 { get; init; }

            string Name { get; init; }

            GamePlatformType PlatformType { get; init; }

            bool SubGame { get; init; }

            string SubGameProviderCode { get; init; }
        }

        internal interface IGameProvider
        {
            bool AllowKickPlayer => false;

            bool AutoClosedRound => true;

            GameProviderBetLimit[] BetLimits { get; }

            string Code { get; }

            ICurrency[] Currencies { get; }

            IGameProviderConfiguration[] GameProviderConfigurations { get; }

            IGame[] Games { get; }

            Guid Id { get; }

            ILanguage[] Languages { get; }

            Dictionary<string, string> MarketTypes => new();

            string Name { get; }

            Dictionary<string, string> SportTypes => new();

            SubGameProvider[] SubGameProviders { get; }

            bool SupportTestPlayer => true;

            bool TransferWallet => false;

            ushort UniqueId { get; }

            PluggableConfigurationBase GetConfiguration();
        }

        internal interface IGameProviderConfiguration
        {
            string ClientId { get; }

            string ConfigurationData => string.Empty;

            string DesktopEndpoint { get; }

            Guid Id { get; }

            string MobileEndpoint { get; }

            string Name { get; }

            string SecretKey { get; }
        }

        internal record BaseCodeMapper : ICodeMapper
        {
            private readonly string _code;
            private readonly string _externalCode;

            protected BaseCodeMapper(string code, string externalCode)
            {
                _code = code;
                _externalCode = externalCode.IsNullOrEmpty() ? code : externalCode;
            }

            string ICodeMapper.Code => _code;
            string ICodeMapper.ExternalCode => _externalCode;
        }

        internal record Game : IGame
        {
            private readonly string _availableCurrencies;
            private readonly string _code;

            [Name("SubGpCode")]
            public string SubGameProviderCode { get; init; }

            [Name("PlatformType")]
            public GamePlatformType PlatformType { get; init; }

            [Name("Name")]
            public string Name { get; init; }

            [Name("Code")]
            public string Code
            {
                get => _code;
                init => _code = value.IsNotNullOrEmpty()
                    ? value
                    : Name.ReplaceApostropheByEmpty()
                        .Replace("&", "And")
                        .Replace("-", "_")
                        .Replace("+", "Plus");
            }

            [Name("ExternalId")]
            public string ExternalId { get; init; }

            [Name("EndpointPath")]
            public string EndpointPath { get; init; }

            [Name("RuleUrl")]
            public string GameRuleUrl { get; init; }

            [Name("Description")]
            public string Description { get; init; }

            [Name("Type")]
            public GameType GameTypeCode { get; init; }

            [Name("HasTurnoverOnTieBet")]
            public bool HasTurnoverOnTieBet { get; init; }

            [Name("SubGame")]
            public bool SubGame { get; init; }

            [Name("AvailableToBrand")]
            public bool AvailableToBrand { get; init; }

            [Name("Html5")]
            public bool Html5 { get; init; }

            [Name("AvailableCurrencies")]
            public string AvailableCurrencies
            {
                get => _availableCurrencies;
                init => _availableCurrencies = value.Replace(";", ",");
            }
        }

        #endregion Interfaces and Base Models
    }
}
