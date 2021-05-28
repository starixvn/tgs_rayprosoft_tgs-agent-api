using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record StartSetPlayerEmailCommand(string PlayerCode, string Email, CultureInfo CultureInfo);
}
