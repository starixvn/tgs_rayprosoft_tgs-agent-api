namespace TGS.Services.Agent.ApplicationServices.Agent
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}
