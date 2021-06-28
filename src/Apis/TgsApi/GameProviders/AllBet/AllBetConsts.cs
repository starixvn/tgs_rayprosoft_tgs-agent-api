namespace TGS.Apis.TgsApi.GameProviders.AllBet
{
    public static class AllBetConsts
    {
        public static class ContextItems
        {
            public static readonly string Agent = "Agent";
        }

        public static class RedisKey
        {
            public static readonly string PlayerCodeByTranId = "AllBetPlayerCodeByTranId";
            public static readonly string BetInfos = "AllBetBetInfos";
        }
    }
}