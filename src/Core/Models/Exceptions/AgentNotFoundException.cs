using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class AgentNotFoundException : Exception
    {
        public AgentNotFoundException(Guid agentId)
            : base($"Agent not found with id: {agentId}")
        {
        }

        public AgentNotFoundException(string userKey, string message)
            : base(string.Format(message, userKey))
        {
        }
    }
}