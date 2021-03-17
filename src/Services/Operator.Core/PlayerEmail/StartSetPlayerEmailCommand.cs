using System.Globalization;

namespace TGS.Services.Operator.PlayerEmail
{
    public record StartSetPlayerEmailCommand(string PlayerCode, string Email, CultureInfo CultureInfo);
}
