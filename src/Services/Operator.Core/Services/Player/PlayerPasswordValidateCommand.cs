using System;

namespace TGS.Services.Operator.Services.Player
{
    public record PlayerPasswordValidateCommand(string PlayerCode, string Password);
}