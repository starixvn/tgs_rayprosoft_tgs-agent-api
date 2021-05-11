using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class AgentLoginExpiredException : Exception
    {
        public AgentLoginExpiredException()
            : base("Agent login token is expired")
        {
        }

        public AgentLoginExpiredException(Guid agentId)
            : base($"Agent({agentId}) login token is expired")
        {
        }
    }
}