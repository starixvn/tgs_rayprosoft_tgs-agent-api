namespace TGS.Services.Agent.Services.Agent
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}
