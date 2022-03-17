using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models
{
    public static class Consts
    {
        public const string Default = "default";
        public const string DefaultCultureCode = "en-US";

        public static readonly GamePlatformType[] AllGamePlatformTypes =
        {
            GamePlatformType.Desktop,
            GamePlatformType.Mobile,
            GamePlatformType.DesktopAndMobile
        };

        public static readonly IDictionary<HealthStatus, int> HealthCheckResultStatusCodes =
            new Dictionary<HealthStatus, int>
            {
                {HealthStatus.Healthy, StatusCodes.Status200OK},
                {HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable},
                {HealthStatus.Degraded, StatusCodes.Status500InternalServerError}
            };

        public static readonly GameType[] Sports =
                        {
            GameType.Sportsbook,
            GameType.VirtualSports,
            GameType.ESports
        };

        public static GameType[] SlotGroup =
        {
            GameType.Fishing,
            GameType.Arcade,
            GameType.Slot,
            GameType.TableGame
        };

        public static GameType[] SportsGroup =
                {
            GameType.Sportsbook,
            GameType.ESports
        };

        public static class AgentTokenExpireSeconds
        {
            public const int AgentLogin = 3600 * 12; // 12 hours
            public const int Jwt = 60 * 60;          // 1 hours
        }

        public static class BackendOffice
        {
            public const string CsrfTokenName = "X-CSRF-TOKEN";

            public static class UserInfo
            {
                public static string BrandKey = "Brand";
                public static string LicenseeKey = "Licensee";
                public static string PortalKey = "Portal";
                public static string ResellerKey = "Reseller";
                public static string RolenameKey = "Rolename";
                public static string RoleTypeKey = "RoleType";
                public static string UserIdKey = "UserId";
                public static string UsernameKey = "Username";
            }
        }

        public static class CachePrefix
        {
            public const string BrandToTgsMap = "BrandToTgsMap";
            public const string GameProviderToTgsMap = "GameProviderToTgsMap";
            public const string TgsToBrandMap = "TgsToBrandMap";
            public const string TgsToGameProviderMap = "TgsToGameProviderMap";
        }

        public static class ContextItems
        {
            public static readonly string AgentToken = "agentToken";
            public static readonly string BrandId = "brandId";
            public static readonly string GameProviderConfigurationId = "gameProviderConfigurationId";
            public static readonly string GameProviderId = "gameProviderId";
            public static readonly string PlatformType = "platformType";
            public static readonly string PlayerCode = "playerCode";
            public static readonly string PlayerToken = "playerToken";
            public static readonly string RequestType = "requestType";
        }

        public static class Db
        {
            public const string MigrationsHistoryTable = "__migrations_history";
            public const string Schema = "dbo";
        }

        public static class EnvironmentVariable
        {
            public const string Environment = "ASPNETCORE_ENVIRONMENT";
            public const string GameProviderCode = "GameProviderCode";
            public const string GameProviderName = "GameProviderName";
        }

        public static class GameProvider
        {
            public const string AllBet = "AB";
            public const string AsiaGaming = "AG";
            public const string Avia = "AVIA";
            public const string BetConstruct = "BC";
            public const string BigGaming = "BG";
            public const string Card365 = "365";
            public const string Evolution = "EVO";
            public const string Habanero = "HABA";
            public const string IMESports = "IMES";
            public const string JDB = "JDB";
            public const string Jili = "JILI";
            public const string KY = "KY";
            public const string LY = "LY";
            public const string PragmaticPlay = "PP";
            public const string Saba = "SABA";
            public const string SBOBET = "SBO";
            public const string SexyGaming = "SG";
            public const string Skywind = "SW";
            public const string TCG = "TCG";
            public const string VR = "VR";
            public const string WGHorse = "WGVIA";
            public const string WGSports = "WGFM";
            public const string WMCasino = "WM";
            public const string TFGaming = "TF";
            public const string DreamGame = "DG";
        }

        public static class LocalizationContext
        {
            public const string GameDescription = "Game.Description";
            public const string GameName = "Game.Name";
            public const string GameProviderDescription = "GameProvider.Description";
            public const string GameProviderName = "GameProvider.Name";
        }

        public static class Nats
        {
            public const string NatsTgsLogQueueName = "TgsLogQueue";
        }

        public static class PromotionGroupType
        {
            public const string Deposit = "Deposit";
            public const string ReliefMoney = "ReliefMoney";
            public const string Turnover = "Turnover";
        }

        public static class TokenExpireSeconds
        {
            public const int Jwt = 60 * 60;
            public const int PlayerLogin = 3600 * 12; // 12 hours
                                                      // 1 hours
        }

        public static class UploadFile
        {
            public const string Banner = "banner";
            public const string ImagePattern = @"\b(http(s?):)([/|:|.|\w|-])*\.(?:jpg|gif|png|jpeg)\b";
            public const string MessengerQRCode = "messengerQRCode";
            public const string Payment = "payment";
            public const string Promotion = "promotion";
            public const string StationLetter = "stationLetter";
            public const string Identity = "identity";

            public static readonly string[] Types =
            {
                StationLetter,
                Promotion,
                MessengerQRCode,
                Banner,
                Payment,
                Identity
            };
        }
    }
}
