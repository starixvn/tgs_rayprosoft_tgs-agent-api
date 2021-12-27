using System;

namespace TGS.Services.Agent.ApplicationServices.PayPassowrd
{
    public record ChangePayPasswordByValidateCodeCommand(
        Guid AgentId,
        string NewPassword,
        string ValidateCode,
        string SecurityKey);
}
