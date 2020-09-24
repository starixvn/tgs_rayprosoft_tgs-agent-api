using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
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
                _externalCode = externalCode ?? code;
            }

            string ICodeMapper.Code => _code;
            string ICodeMapper.ExternalCode => _externalCode;
        }

        internal record BaseGameProvider
        {
            private static readonly string Namespace = "TGS.Core.DataServices.DataInitializers";

            protected IGame[] Games { get; }

            public BaseGameProvider(string gameProviderCode)
            {
                if (gameProviderCode.IsNullOrEmpty())
                    return;

                var csvName = $"{Namespace}.Games.{gameProviderCode}.csv";
                using var stream = typeof(DataInitializer).Assembly.GetManifestResourceStream(csvName);
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                Games = csv.GetRecords<Game>().Cast<IGame>().ToArray();
            }
        }

        internal record Game : IGame
        {
            private string _code;
            private string _name;

            public Guid Id => Guid.NewGuid();

            [Name("Name")]
            public string Name
            {
                get { return _name; }
                init { _name = value; }
            }

            [Name("Code")]
            public string Code
            {
                get { return _code; }
                init
                {
                    _code = value ?? _name.ReplaceApostropheByEmpty()
                                          .Replace("&", "And")
                                          .Replace("-", "_")
                                          .Replace("+", "Plus");
                }
            }

            [Name("ExternalId")]
            public string ExternalId { get; init; }

            [Name("EndpointPath")]
            public string EndpointPath { get; init; }

            [Name("GameRuleUrl")]
            public string GameRuleUrl { get; init; }

            [Name("Description")]
            public string Description { get; init; }

            [Name("GameTypeCode")]
            public GameType GameTypeCode { get; init; }

            [Name("PlatformType")]
            public PlatformType PlatformType { get; init; }

            [Name("HasTurnoverOnTieBet")]
            public bool HasTurnoverOnTieBet { get; init; }

            [Name("SubGame")]
            public bool SubGame { get; init; }
        }

        #endregion Interfaces and Base Models
    }
}