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

        public const string Environment = "ASPNETCORE_ENVIRONMENT";
        public const string DefualtCultureCode = "en-US";

        public static class Player
        {
            public const int LoginExpirationSeconds = 3600 * 24; // 1d
        }
    }
}