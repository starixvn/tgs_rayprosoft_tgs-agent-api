using System;
using TGS.Core.Models.Enums;

#nullable enable

namespace TGS.Apis.AgentApi.Models
{
    public class GetAgentPlayersRequest
    {
        public DateTimeOffset? BeginRegisteredOn { get; init; }

        public DateTimeOffset? EndRegisteredOn { get; init; }

        public string? RegisterHost { get; init; }

        public PlayerStatus? Status { get; init; }

        public string? Username { get; init; }

        public Guid? VipId { get; init; }
    }
}
