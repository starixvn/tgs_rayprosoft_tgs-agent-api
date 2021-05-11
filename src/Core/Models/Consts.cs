using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models
{
    public static class Consts
    {
        public const string DefaultCultureCode = "en-US";
        public const string Default = "default";

        public static readonly GameType[] Sports = {GameType.Sportsbook, GameType.VirtualSports, GameType.ESports};

        public static readonly GamePlatformType[] AllGamePlatformTypes =
        {
            GamePlatformType.Desktop, GamePlatformType.Mobile, GamePlatformType.DesktopAndMobile
        };

        public static readonly IDictionary<HealthStatus, int> HealthCheckResultStatusCodes =
            new Dictionary<HealthStatus, int>
            {
                {HealthStatus.Healthy, StatusCodes.Status200OK},
                {HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable},
                {HealthStatus.Degraded, StatusCodes.Status500InternalServerError}
            };

        public static class CachePrefix
        {
            public const string GameProviderToTgsMap = "GameProviderToTgsMap";
            public const string TgsToGameProviderMap = "TgsToGameProviderMap";
            public const string BrandToTgsMap = "BrandToTgsMap";
            public const string TgsToBrandMap = "TgsToBrandMap";
        }

        public static class Nats
        {
            public const string NatsTgsLogQueueName = "TgsLogQueue";
        }

        public static class Db
        {
            public const string MigrationsHistoryTable = "__migrations_history";
            public const string Schema = "dbo";
        }

        public static class TokenExpireSeconds
        {
            public const int PlayerLogin = 3600 * 12; // 12 hours
            public const int Jwt = 60 * 60;           // 1 hours
        }

        public static class AgentTokenExpireSeconds
        {
            public const int AgentLogin = 3600 * 12; // 12 hours
            public const int Jwt = 60 * 60;          // 1 hours
        }

        public static class GameProvider
        {
            public const string RedTiger = "RT";
            public const string SBOBET = "SBO";
            public const string Habanero = "HABA";
            public const string PragmaticPlay = "PP";
            public const string AsiaGaming = "AG";
            public const string Saba = "SABA";
        }

        public static class EnvironmentVariable
        {
            public const string Environment = "ASPNETCORE_ENVIRONMENT";
            public const string GameProviderCode = "GameProviderCode";
            public const string GameProviderName = "GameProviderName";
        }

        public static class ContextItems
        {
            public static readonly string BrandId = "brandId";
            public static readonly string GameProviderId = "gameProviderId";
            public static readonly string GameProviderConfigurationId = "gameProviderConfigurationId";
            public static readonly string PlatformType = "platformType";
            public static readonly string AgentToken = "agentToken";
            public static readonly string PlayerToken = "playerToken";
            public static readonly string PlayerCode = "playerCode";
            public static readonly string RequestType = "requestType";
        }

        public static class LocalizationContext
        {
            public const string GameProviderName = "GameProvider.Name";
            public const string GameProviderDescription = "GameProvider.Description";
            public const string GameName = "Game.Name";
            public const string GameDescription = "Game.Description";
        }

        public static class BackendOffice
        {
            public const string CsrfTokenName = "X-CSRF-TOKEN";

            public static class UserInfo
            {
                public static string UsernameKey = "Username";
                public static string RolenameKey = "Rolename";
                public static string RoleTypeKey = "RoleType";
                public static string ResellerKey = "Reseller";
                public static string LicenseeKey = "Licensee";
                public static string BrandKey = "Brand";
                public static string PortalKey = "Portal";
            }
        }
    }
}