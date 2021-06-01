using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record StartSetPlayerPhoneCommand(string PlayerCode, string Phone, CultureInfo CultureInfo);
}
