using System;

namespace TGS.Services.Agent
{
    public class AgentNotFoundException : Exception
    {
        public AgentNotFoundException(string email)
            : base($"The email ({email}) can't find the agent info.")
        {
        }
    }
}