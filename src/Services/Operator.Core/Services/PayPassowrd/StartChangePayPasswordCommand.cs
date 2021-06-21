using System.Globalization;

namespace TGS.Services.Operator.Services.PayPassowrd
{
    public record StartChangePayPasswordCommand(string PlayerCode, CultureInfo? CultureInfo);
}
