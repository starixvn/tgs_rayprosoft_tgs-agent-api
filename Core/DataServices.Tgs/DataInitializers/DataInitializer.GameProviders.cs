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
    partial class DataInitializer
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

        internal interface IGameProvider
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            IGameProviderConfiguration[] GameProviderConfigurations { get; }
            ICurrency[] Currencies { get; }
            ILanguage[] Languages { get; }
            IGame[] Games { get; }
            bool AutoClosedRound => true;
            bool SupportTestPlayer => true;
            bool AllowKickPlayer => false;
            short UniqueId { get; }
            GameProviderBetLimit[] BetLimits { get; }
            Dictionary<string, string> SportTypes => new();
            Dictionary<string, string> MarketTypes => new();
            PluggableConfigurationBase GetConfiguration();
        }

        internal interface IGameProviderConfiguration
        {
            Guid Id { get; }
            string Name { get; }
            string ClientId { get; }
            string SecretKey { get; }
            string DesktopEndpoint { get; }
            string MobileEndpoint { get; }
            string ConfigurationData => string.Empty;
        }

        internal interface IGame
        {
            string Name { get; init; }
            string Code { get; init; }
            string ExternalId { get; init; }
            string EndpointPath { get; init; }
            string GameRuleUrl { get; init; }
            string Description { get; init; }
            GameType GameTypeCode { get; init; }
            PlatformType PlatformType { get; init; }
            bool HasTurnoverOnTieBet { get; init; }
            bool SubGame { get; init; }
            bool Html5 { get; init; }
            string AvailableCurrencies { get; init; }
        }

        internal interface ICodeMapper
        {
            string Code { get; }
            string ExternalCode { get; }
        }

        internal record BaseCodeMapper : ICodeMapper
        {
            private readonly string _code;
            private readonly string _externalCode;

            public BaseCodeMapper(string code, string externalCode)
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

            [Name("GameType")]
            public GameType GameTypeCode { get; init; }

            [Name("Platform")]
            public PlatformType PlatformType { get; init; }

            [Name("HasTurnoverOnTieBet")]
            public bool HasTurnoverOnTieBet { get; init; }

            [Name("SubGame")]
            public bool SubGame { get; init; }

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