namespace TGS.Services.Agent.Agent
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}