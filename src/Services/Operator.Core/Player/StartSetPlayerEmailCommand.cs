using System.Globalization;

namespace TGS.Services.Operator.Player
{
    public record StartSetPlayerEmailCommand(string PlayerCode, string Email, CultureInfo CultureInfo);
}
