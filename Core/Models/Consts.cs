namespace TGS.Core.Models
{
    public static class Consts
    {
        public static class CachePrefix
        {
            public const string GameProviderToTgsMap = "GameProviderToTgsMap";
            public const string TgsToGameProviderMap = "TgsToGameProviderMap";
        }

        public static class Nats
        {
            public const string NatsTgsLogQueueName = "TgsLogQueue";
        }

        public static class Db
        {
            public const string MigrationsHistoryTable = "_MigrationsHistory";
            public const string Schema = "dbo";
        }

        public static class Player
        {
            public const int LoginExpirationSeconds = 3600 * 12; // 12 hours
        }

        public static class GameProvider
        {
            public const string RedTiger = "RT";
            public const string SBOBET = "SBO";
        }

        public static class EnvironmentVariable
        {
            public const string Environment = "ASPNETCORE_ENVIRONMENT";
            public const string GameProviderCode = "GameProviderCode";
            public const string GameProviderName = "GameProviderName";
        }

        public static class ContextItems
        {
            public const string LicenseeId = "licenseeId";
            public const string GameProviderId = "gameProviderId";
            public const string GameProviderConfigurationId = "gameProviderConfigurationId";
            public const string PlatformType = "platformType";
        }

        public const string DefualtCultureCode = "en-US";
        public const string Default = "default";
    }
}