using System;

namespace TGS.Services.Operator.Services.Player
{
    public record ChangePasswordQuery(
        Guid BrandId,
        string PlayerCode,
        string OldPassword,
        string NewPassword);
}