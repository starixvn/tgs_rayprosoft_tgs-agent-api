using System;

namespace TGS.Apis.AgentApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AuthorizeAgentTokenAttribute : Attribute
    {
        public bool AllowNullOrEmpty { get; init; }

        public bool NeedAuthorize { get; init; }
    }
}
