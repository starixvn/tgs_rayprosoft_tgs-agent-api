using System;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models
{
    public class GetAccountChangesRequest
    {
        [FromQuery(Name = "from")]
        public DateTimeOffset From { get; set; }

        [FromQuery(Name = "to")]
        public DateTimeOffset To { get; set; }

        [FromQuery(Name = "transactiontype")]
        public TransactionType? TransactionType { get; set; }
    }
}
