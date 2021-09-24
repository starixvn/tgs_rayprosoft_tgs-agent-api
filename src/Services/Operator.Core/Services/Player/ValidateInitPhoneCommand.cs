using System;

namespace TGS.Services.Operator.Services.Player
{
    public record ValidateInitPhoneCommand(Guid BrandId, string Username, string Password);
}