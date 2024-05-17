using System;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record GetAgentBalanceQuery(Guid WalletId, Guid AgentId);