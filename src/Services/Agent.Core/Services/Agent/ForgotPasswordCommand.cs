using System.Globalization;

namespace TGS.Services.Agent.Services.Agent
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Email);
}
