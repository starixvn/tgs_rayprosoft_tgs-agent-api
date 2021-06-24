using System;

namespace TGS.Services.Operator.Services.Player
{
    public record GetChangePasswordQuery(
        Guid BrandId,
        string PlayerCode,
        string OldPassword,
        string NewPassword);
}