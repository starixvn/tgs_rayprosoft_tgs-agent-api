using System;
using System.Collections.Generic;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero
{
    public static class HabaneroConsts
    {
        public static readonly IReadOnlyDictionary<RequestType, Type> RequestTypeMapper =
            new Dictionary<RequestType, Type>
            {
                {RequestType.PlayerDetailRequest, typeof(AuthorizePlayerRequest)},
                {RequestType.FundTransferRequest, typeof(FundTransferRequest)},
                {RequestType.QueryRequest, typeof(QueryRequest)}
            };
    }
}