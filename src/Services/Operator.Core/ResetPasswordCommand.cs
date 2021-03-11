namespace TGS.Services.Operator
{
    public record ResetPasswordCommand(string Token, string ValidateCode, string NewPassword);
}
