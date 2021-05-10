using System.Globalization;

namespace TGS.Services.Operator.Player
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Email);
}