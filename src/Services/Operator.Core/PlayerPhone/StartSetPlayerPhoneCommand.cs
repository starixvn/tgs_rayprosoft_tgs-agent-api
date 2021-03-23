using System.Globalization;

namespace TGS.Services.Operator.PlayerPhone
{
    public record StartSetPlayerPhoneCommand(string PlayerCode, string Phone, CultureInfo CultureInfo);
}
