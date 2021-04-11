using System;
using System.Collections.Generic;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.JDB
{
    public static class JDBConsts
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