using System.Globalization;

namespace TGS.Services.Operator.PlayerPassword
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Phone);
}
