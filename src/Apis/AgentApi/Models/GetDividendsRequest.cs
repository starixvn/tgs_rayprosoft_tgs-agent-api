using System;

namespace TGS.Apis.AgentApi.Models
{
    public record GetDividendsRequest : PagingRequest
    {
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}