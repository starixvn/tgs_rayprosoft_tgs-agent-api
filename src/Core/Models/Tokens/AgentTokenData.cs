using System;

namespace TGS.Core.Models.Tokens
{
    public record AgentTokenData
    {
        public Guid AgentId { get; init; }
        public Guid LoginRecordId { get; init; }
        public DateTimeOffset ExpiredOn { get; init; }
    }
}