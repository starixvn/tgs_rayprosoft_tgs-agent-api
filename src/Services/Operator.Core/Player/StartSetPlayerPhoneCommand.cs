using System.Globalization;

namespace TGS.Services.Operator.Player
{
    public record StartSetPlayerPhoneCommand(string PlayerCode, string Phone, CultureInfo CultureInfo);
}
