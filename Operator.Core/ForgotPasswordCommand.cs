using System.Globalization;

namespace TGS.Services.Operator
{
    public record ForgotPasswordCommand(CultureInfo CultureInfo, string Phone);
}
