using System;

namespace TGS.Services.Agent.Exceptions;

public class AgentNotFoundException : Exception
{
    public AgentNotFoundException(string info)
        : base($"The ({info}) can't find the agent info.")
    {
    }
}
