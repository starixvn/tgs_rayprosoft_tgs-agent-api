namespace TGS.Services.Operator.PlayerPassword
{
    public record ResetPasswordCommand(string Token, string ValidateCode, string NewPassword);
}
