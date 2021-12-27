using System;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    record EmailValidateData(string Email, Guid AgentId);
}
