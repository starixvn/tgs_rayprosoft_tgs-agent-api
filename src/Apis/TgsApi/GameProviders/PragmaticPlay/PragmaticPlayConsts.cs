using System;
using System.Collections.Generic;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay
{
    public static class PragmaticPlayConsts
    {
        public static readonly IReadOnlyDictionary<string, string> CampaignTypeMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"T", "Tournament"},
                {"C", "Chat game winning in bingo"},
                {"B", "Bonus award"}
            };

        public static class Parameters
        {
            public static readonly string Hash = "hash";
            public static readonly string ProviderId = "providerId";
            public static readonly string Token = "token";
            public static readonly string UserId = "userId";
            public static readonly string BypassHashVerify = "bypassHashVerify";
        }
    }
}