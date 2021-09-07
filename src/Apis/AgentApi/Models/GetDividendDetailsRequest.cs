using System;

namespace TGS.Apis.AgentApi.Models
{
    public record GetDividendDetailsRequest : PagingRequest
    {
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}