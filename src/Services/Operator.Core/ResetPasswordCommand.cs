namespace TGS.Services.Operator
{
    public record ResetPasswordCommand(string PlayerCode, string NewPassword);
}
