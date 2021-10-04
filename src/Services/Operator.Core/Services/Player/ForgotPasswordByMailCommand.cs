using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record ForgotPasswordByMailCommand(CultureInfo CultureInfo, string Email);
}
