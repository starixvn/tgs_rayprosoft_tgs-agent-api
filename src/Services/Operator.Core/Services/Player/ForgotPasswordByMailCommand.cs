using System;
using System.Globalization;

namespace TGS.Services.Operator.Services.Player
{
    public record ForgotPasswordByMailCommand(
        Guid BrandId,
        CultureInfo CultureInfo,
        string Email);
}
