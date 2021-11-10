using System;
using System.Globalization;

namespace TGS.Services.Agent.Services.Agent
{
    public record ForgotPasswordCommand(Guid BrandId, CultureInfo CultureInfo, string Email);
}
