namespace TGS.Services.Operator.PlayerPassword
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}
