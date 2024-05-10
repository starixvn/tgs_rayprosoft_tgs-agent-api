using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

public record AgentLoginRecordInfo(
    Guid LoginRecordId,
    Guid AgentId,
    LoginStatus Status,
    DateTimeOffset CreatedOn,
    DateTimeOffset ExpiredOn);