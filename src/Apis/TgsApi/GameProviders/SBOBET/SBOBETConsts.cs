namespace TGS.Apis.TgsApi.GameProviders.SBOBET
{
    public static class SBOBETConsts
    {
        public static class RedisKey
        {
            public static readonly string BetTransaction = "SBOBetTrx";
            public static readonly string RollbackTransaction = "SBORollbackTrx";
        }
    }
}