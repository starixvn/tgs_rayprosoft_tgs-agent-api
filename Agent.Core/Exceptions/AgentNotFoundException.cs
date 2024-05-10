using System;

namespace TGS.Services.Agent.Exceptions;

public class AgentNotFoundException(string info) : Exception($"The ({info}) can't find the agent info.");
