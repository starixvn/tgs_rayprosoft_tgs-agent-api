using System;
using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record ForgotPasswordBySmsCommand(
        Guid BrandId,
        CultureInfo CultureInfo,
        string AreaCode,
        string Phone);
}
