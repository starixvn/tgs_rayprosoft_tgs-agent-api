namespace TGS.Services.Operator.Player
{
    public record ResetPasswordCommand(string SecurityKey, string ValidateCode, string NewPassword);
}
