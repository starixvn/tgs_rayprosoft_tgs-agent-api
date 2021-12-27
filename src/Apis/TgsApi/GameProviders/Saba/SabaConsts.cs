namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public static class SabaConsts
    {
        public static readonly string ExternalGameId = "1";

        public static class ContextItems
        {
            public static readonly string IsTestEnvironment = "IsTestEnv";
        }

        public static class RedisKey
        {
            public static readonly string LockAmount = "SabaLockAmount";
            public static readonly string SabaTxIdMap = "SabaTxIdMap";
            public static readonly string Token = "SabaTsId";
        }
    }
}
