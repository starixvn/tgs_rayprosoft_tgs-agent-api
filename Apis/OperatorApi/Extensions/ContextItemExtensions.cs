using System;
using System.Collections.Generic;
using TGS.Core.Models.Tokens;
using static TGS.Core.Models.Consts.ContextItems;

namespace TGS.Apis.OperatorApi.Extensions
{
    internal static class ContextItemExtensions
    {
        public static Guid GetLicenseeId(this IDictionary<object, object> items) =>
            items.TryGetValue(LicenseeId, out var v) && v != null ? (Guid) v : Guid.Empty;

        public static PlayerTokenData GetPlayerTokenData(this IDictionary<object, object> items) =>
            items.TryGetValue(PlayerToken, out var v) && v != null ? (PlayerTokenData) v : null;
    }
}