using System;

namespace TGS.Apis.AgentApi.Attributes
{
    /// <summary>
    ///     When agent status is disabled, agent can't use this api
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckAgentEnabledAttribute : Attribute
    {
    }
}