using System.Collections.Generic;
using TGS.Shared.Game.Models.LY.Enums;

namespace TGS.Shared.Game.ApiClients.LY
{
    public class LYActionMapper
    {
        public static readonly IReadOnlyDictionary<LYAction, int> Mapper = new Dictionary<LYAction, int>() {
            { LYAction.GetGameUrl, 0 },
            { LYAction.GetBalance, 7 },
            { LYAction.FundOut, 2 },
            { LYAction.FundIn, 3 },
            { LYAction.FundChecking, 4 },
            { LYAction.Kick, 8 },
            { LYAction.GameRecord, 6 }
            };
    }
}
