using System.Collections.Generic;
using TGS.Shared.Game.Models.Avia;

namespace TGS.Shared.Game.ApiClients.Avia
{
    internal class AviaMapper
    {
        internal static readonly IReadOnlyDictionary<AviaAction, string> AviaActionMapper = new Dictionary<AviaAction, string>() {
            {AviaAction.Login, "/api/user/login"},
            {AviaAction.Register, "/api/user/register"},
            {AviaAction.Balance, "/api/user/balance"},
            {AviaAction.TransferOut, "/api/user/transfer"},
            {AviaAction.TransferIn, "/api/user/transfer"},
            {AviaAction.Transferinfo, "/api/user/transferinfo"},
            {AviaAction.GameRecord, "/api/log/get"},
            {AviaAction.Logout, "/api/user/logout"}
        };
    }
}
