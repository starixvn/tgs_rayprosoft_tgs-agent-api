using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models;

public record GetRoundsReportRequest
{        
    public string Username { get; init; }        

    public Guid? RoundId { get; init; }

    public string ExternalRoundId { get; init; }

    public Guid? GameProviderId { get; init; }

    public ClientRoundStatus? Status { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public DateTimeOffset? ClosedFrom { get; init; }

    public DateTimeOffset? ClosedTo { get; init; }
}
