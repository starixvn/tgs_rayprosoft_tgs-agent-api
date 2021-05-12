using System;
using System.Collections.Generic;
using TGS.Core.Models.Tokens;
using static TGS.Core.Models.Consts.ContextItems;

namespace TGS.Apis.AgentApi.Extensions
{
    internal static class ContextItemExtensions
    {
        public static Guid GetBrandId(this IDictionary<object, object> items) =>
            items.TryGetValue(BrandId, out var v) && v != null ? (Guid)v : Guid.Empty;

        public static AgentTokenData GetAgentTokenData(this IDictionary<object, object> items) =>
            items.TryGetValue(AgentToken, out var v) && v != null ? (AgentTokenData)v : null;
    }
}