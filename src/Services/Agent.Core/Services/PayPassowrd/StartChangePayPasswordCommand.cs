using System;
using System.Globalization;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    public record StartChangePayPasswordCommand(Guid AgentId, CultureInfo? CultureInfo);
}
