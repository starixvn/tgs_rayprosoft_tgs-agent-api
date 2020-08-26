namespace TGS.Core.Models
{
    public class Consts
    {
        public class CachePrefix
        {
            public const string GameProviderToTgsMap = "GameProviderToTgsMap";
            public const string TgsToGameProviderMap = "TgsToGameProviderMap";
        }

        public class Nats
        {
            public const string NatsTgsLogQueueName = "TgsLogQueue";
        }


        public const string CsrfTokenName = "X-XSRF-TOKEN";
    }
}
