using System;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    public record ChangePayPasswordByValidateCodeCommand(
        Guid AgentId,
        string NewPassword,
        string ValidateCode,
        string SecurityKey);
}
