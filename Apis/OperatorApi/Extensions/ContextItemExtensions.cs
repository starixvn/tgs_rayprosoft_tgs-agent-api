using System;
using System.Collections.Generic;
using static TGS.Core.Models.Consts.ContextItems;

namespace TGS.Apis.OperatorApi.Extensions
{
    internal static class ContextItemExtensions
    {
        public static Guid GetLicenseeId(this IDictionary<object, object> items)
        {
            return items.TryGetValue(LicenseeId, out var v) && v != null ? (Guid) v : Guid.Empty;
        }
    }
}