using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public static class SabaConsts
    {
        public static readonly string ExternalGameId = "1";

        public static readonly IReadOnlyDictionary<string, GameActionType> GameActionTypeMap =
            new Dictionary<string, GameActionType>(StringComparer.OrdinalIgnoreCase)
            {
                {"half won", GameActionType.WinBet},
                {"half lose", GameActionType.WinBet},
                {"won", GameActionType.WinBet},
                {"lose", GameActionType.LoseBet},
                {"draw", GameActionType.TieBet},
                {"void", GameActionType.CancelTransaction},
                {"refund", GameActionType.CancelTransaction},
                {"reject", GameActionType.CancelTransaction}
            };

        public static class RedisKey
        {
            public static readonly string Token = "SabaTsId";
            public static readonly string LockAmount = "SabaLockAmount";
            public static readonly string SabaTxIdMap = "SabaTxIdMap";
        }

        public static class ContextItems
        {
            public static readonly string IsTestEnvironment = "IsTestEnv";
        }
    }
}