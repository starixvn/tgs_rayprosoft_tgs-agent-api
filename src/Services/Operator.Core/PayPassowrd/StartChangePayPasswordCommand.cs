using System.Globalization;

namespace TGS.Services.Operator.PayPassowrd
{
    public record StartChangePayPasswordCommand(string PlayerCode, CultureInfo? CultureInfo);
}
