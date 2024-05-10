using System;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal record EmailValidateData(string Email, Guid AgentId);