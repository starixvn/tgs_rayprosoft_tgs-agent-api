using System;

namespace TGS.Services.Agent
{
    public class AgentOptions
    {
        public Uri ValidateCodeBaseUrl { get; set; } = new Uri("https://localhost:5001");
    }
}