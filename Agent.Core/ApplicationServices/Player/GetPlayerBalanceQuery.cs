using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record GetPlayerBalanceQuery(Guid AgentId, string PlayerCode);