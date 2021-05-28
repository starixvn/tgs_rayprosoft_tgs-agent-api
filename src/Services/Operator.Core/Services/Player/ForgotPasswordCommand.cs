using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Email);
}
