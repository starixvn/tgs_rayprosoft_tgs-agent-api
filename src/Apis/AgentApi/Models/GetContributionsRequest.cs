using System;

namespace TGS.Apis.AgentApi.Models
{
    public record GetContributionsRequest : PagingRequest
    {
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}