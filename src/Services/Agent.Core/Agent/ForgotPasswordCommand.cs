using System.Globalization;

namespace TGS.Services.Agent.Agent
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Email);
}