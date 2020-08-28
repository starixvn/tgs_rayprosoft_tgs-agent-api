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

        public const string CsrfTokenName = "X-XSRF-TOKEN";
        public const string Environment = "ASPNETCORE_ENVIRONMENT";
    }
}