namespace TGS.Services.Operator.Services.Player
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}
