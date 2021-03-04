using System;
using System.Collections.Generic;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero
{
    public static class HabaneroConsts
    {
        public static readonly IReadOnlyDictionary<string, Type> RequestTypeMapper =
            new Dictionary<string, Type>
            {
                {"playerdetailrequest", typeof(AuthorizePlayerRequest)},
                {"fundtransferrequest", typeof(FundTransferRequest)},
                {"queryrequest", typeof(QueryRequest)}
            };
    }
}