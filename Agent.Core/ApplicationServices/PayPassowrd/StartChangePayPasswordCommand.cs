using System;
using System.Globalization;

namespace TGS.Services.Agent.ApplicationServices.PayPassowrd;

public record StartChangePayPasswordCommand(Guid AgentId, CultureInfo? CultureInfo);