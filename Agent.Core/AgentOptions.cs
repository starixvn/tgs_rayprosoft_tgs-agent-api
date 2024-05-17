using System;

namespace TGS.Services.Agent;

public class AgentOptions
{
    public Uri SmsBaseUrl { get; set; } = new("https://localhost:5002");

    public Uri ValidateCodeBaseUrl { get; set; } = new("https://localhost:5001");
}